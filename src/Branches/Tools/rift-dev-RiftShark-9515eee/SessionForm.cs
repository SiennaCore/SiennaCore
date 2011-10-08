using Lyquidity.Controls.ExtendedListViews;
using OpenSSL.Core;
using OpenSSL.Crypto;
using SharpPcap.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace RiftShark
{
    public partial class SessionForm : DockContent
    {
        private Dictionary<bool, byte[]> mClientPrivateKeys = null;
        private bool mTerminated = false;
        private ushort mLocalPort = 0;
        private ushort mRemotePort = 0;
        private uint mOutboundSequence = 0;
        private uint mInboundSequence = 0;
        private SortedDictionary<uint, byte[]> mOutboundBuffer = new SortedDictionary<uint, byte[]>();
        private SortedDictionary<uint, byte[]> mInboundBuffer = new SortedDictionary<uint, byte[]>();
        private RiftStream mOutboundStream = new RiftStream(true);
        private RiftStream mInboundStream = new RiftStream(false);
        private bool mIsCharacterSession = false;
        private List<RiftPacket> mPackets = new List<RiftPacket>();
        private BigNumber mClientPrivateKey = null;
        private BigNumber mServerPublicKey = null;
        private byte[] mSharedSecretKey = null;

        private BigNumber mGenerator = new BigNumber((uint)2);
        private BigNumber mModulus = BigNumber.FromArray(new byte[]
        {
            0xEF, 0x04, 0x19, 0x98, 0x21, 0x99, 0x68, 0x6A, 0x8C, 0xB5, 0xCA, 0xBA, 0xC5, 0x31, 0xD2, 0xC0,
            0x70, 0xCF, 0x07, 0x77, 0xE8, 0x8F, 0xFD, 0x01, 0x5B, 0x71, 0x64, 0xBD, 0xCC, 0x7B, 0x91, 0x66,
            0x6E, 0x74, 0x11, 0x64, 0x6C, 0x3D, 0x68, 0x48, 0x8C, 0x7C, 0x0C, 0xB3, 0x86, 0x8F, 0x94, 0x5F,
            0x55, 0xEF, 0x20, 0x3C, 0x78, 0xB2, 0x7F, 0x2F, 0xDE, 0x74, 0x95, 0x42, 0x0B, 0x6F, 0x1C, 0x89,
            0x89, 0xDF, 0x69, 0xCC, 0xDD, 0x9E, 0x78, 0x6F, 0x81, 0xE1, 0x0C, 0xAD, 0xE8, 0x7F, 0x3B, 0x6E,
            0x59, 0x7E, 0x71, 0x4B, 0x3C, 0xE1, 0x54, 0x52, 0x99, 0x41, 0x58, 0x2D, 0x99, 0x96, 0xE8, 0xEE,
            0xF5, 0xEB, 0x68, 0x14, 0x72, 0x18, 0x5B, 0xD2, 0x86, 0xE7, 0x3A, 0xFB, 0x57, 0xD1, 0x81, 0x0F,
            0xA2, 0xAC, 0xED, 0x65, 0x33, 0x27, 0x61, 0x42, 0xE0, 0x24, 0xB3, 0x49, 0x7B, 0xAA, 0x40, 0x03
        });

        private string mFilename = null;
        private List<Plugin> mPlugins = new List<Plugin>();

        public SessionForm()
        {
            InitializeComponent();
        }

        public void CreatePlugins(List<Type> pPluginTypes)
        {
            pPluginTypes.ForEach(t => mPlugins.Add((Plugin)t.InvokeMember("", BindingFlags.CreateInstance, null, null, new object[] { })));
        }

        public void ProgressUpdate()
        {
            try
            {
                mStatusProgressBar.PerformStep();
                mStatusProgressBar.ForeColor = Color.FromArgb(mStatusProgressBar.Value * 2, mStatusProgressBar.Value * 2, mStatusProgressBar.Value * 2);
                if (mStatusProgressBar.Value == mStatusProgressBar.Maximum)
                {
                    mStatusProgressBar.Value = 0;
                    mStatusProgressStateLabel.Text = "Done";
                }
                else mStatusProgressStateLabel.Text = ((mStatusProgressBar.Value * 100) / mStatusProgressBar.Maximum) + "%";
            }
            catch (Exception) { }
            Application.DoEvents();
        }

        internal void ImportClientPrivateKeys(Dictionary<bool, byte[]> pClientPrivateKeys) { mClientPrivateKeys = new Dictionary<bool,byte[]>(pClientPrivateKeys); }

        internal bool MatchTCPPacket(TCPPacket pTCPPacket)
        {
            if (mTerminated) return false;
            if (pTCPPacket.SourcePort == mLocalPort && pTCPPacket.DestinationPort == mRemotePort) return true;
            if (pTCPPacket.SourcePort == mRemotePort && pTCPPacket.DestinationPort == mLocalPort) return true;
            return false;
        }

        internal void BufferTCPPacket(TCPPacket pTCPPacket)
        {
            if (pTCPPacket.Fin || pTCPPacket.Rst)
            {
                mTerminated = true;
                return;
            }
            if (mOutboundSequence == 0)
            {
                mLocalPort = (ushort)pTCPPacket.SourcePort;
                mRemotePort = (ushort)pTCPPacket.DestinationPort;
                mOutboundSequence = (uint)pTCPPacket.SequenceNumber + 1;
                Text  = "Port " + mLocalPort.ToString();
            }
            if (mInboundSequence == 0 && pTCPPacket.SourcePort == mRemotePort) mInboundSequence = (uint)pTCPPacket.SequenceNumber + 1;
            if (pTCPPacket.PayloadDataLength == 0) return;
            if (pTCPPacket.SourcePort == mLocalPort) ProcessTCPPacket(pTCPPacket, ref mOutboundSequence, mOutboundBuffer, mOutboundStream);
            else ProcessTCPPacket(pTCPPacket, ref mInboundSequence, mInboundBuffer, mInboundStream);
        }

        private void ProcessTCPPacket(TCPPacket pTCPPacket, ref uint pSequence, SortedDictionary<uint, byte[]> pBuffer, RiftStream pStream)
        {
            if (pTCPPacket.SequenceNumber > pSequence) pBuffer[(uint)pTCPPacket.SequenceNumber] = pTCPPacket.TCPData;
            if (pTCPPacket.SequenceNumber < pSequence)
            {
                int difference = (int)(pSequence - pTCPPacket.SequenceNumber);
                byte[] data = pTCPPacket.TCPData;
                if (data.Length > difference)
                {
                    pStream.Append(data, difference, data.Length - difference);
                    pSequence += (uint)(data.Length - difference);
                }
            }
            else if (pTCPPacket.SequenceNumber == pSequence)
            {
                byte[] data = pTCPPacket.TCPData;
                pStream.Append(data);
                pSequence += (uint)data.Length;

                bool found;
                do
                {
                    SortedDictionary<uint, byte[]>.Enumerator enumerator = pBuffer.GetEnumerator();
                    if ((found = (enumerator.MoveNext() && enumerator.Current.Key <= pSequence)))
                    {
                        int difference = (int)(pSequence - enumerator.Current.Key);
                        if (enumerator.Current.Value.Length > difference)
                        {
                            pStream.Append(enumerator.Current.Value, difference, enumerator.Current.Value.Length - difference);
                            pSequence += (uint)(enumerator.Current.Value.Length - difference);
                        }
                        pBuffer.Remove(enumerator.Current.Key);
                    }
                }
                while (found);
            }

            RiftPacket packet;
            while ((packet = pStream.Read(pTCPPacket.Timeval.Date)) != null)
            {
                AddPacket(packet);
                if (packet.Opcode == 0x01B7)
                {
                    mIsCharacterSession = true;
                }
                else if (packet.Opcode == 0x040B)
                {
                    RiftPacketField fieldServerPublicKey;
                    if (packet.GetFieldByIndex(out fieldServerPublicKey, 1) &&
                        fieldServerPublicKey.Type == ERiftPacketFieldType.ByteArray &&
                        fieldServerPublicKey.Value.Bytes.Length == 128)
                    {
                        if (mClientPrivateKeys == null)
                        {
                            DateTime started = DateTime.Now;
                            while (!Program.LiveKeys.ContainsKey(mIsCharacterSession) && DateTime.Now.Subtract(started).TotalSeconds < 10) Thread.Sleep(1);
                            if (Program.LiveKeys.ContainsKey(mIsCharacterSession)) mClientPrivateKeys = Program.LiveKeys;
                            else
                            {
                                MessageBox.Show(this, "The required key was unable to be found for some reason, let the developers know this happened.", "Key Grab Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                mTerminated = true;
                                return;
                            }
                        }
                        mClientPrivateKey = BigNumber.FromArray(mClientPrivateKeys[mIsCharacterSession]);
                        mServerPublicKey = BigNumber.FromArray(fieldServerPublicKey.Value.Bytes);
                        DH dh = new DH(mModulus, mGenerator, BigNumber.One, mClientPrivateKey);
                        mSharedSecretKey = dh.ComputeKey(mServerPublicKey);
                    }
                }
                else if (packet.Opcode == 0x19)
                {
                    pStream.EnableInflater();
                    if (packet.Outbound)
                    {
                        mInboundStream.EnableEncryption(mSharedSecretKey);
                        mOutboundStream.EnableEncryption(mSharedSecretKey);
                    }
                }
            }
        }

        private void AddPacket(RiftPacket pPacket, bool pRefreshing = false)
        {
            if (!pRefreshing) mPackets.Add(pPacket);
            mPacketView.BeginUpdate();

            TreeListNode node = new TreeListNode();
            node.Tag = pPacket;
            node.Text = pPacket.Timestamp.ToLongTimeString();
            node.SubItems.Add(pPacket.Outbound ? "Outbound" : "Inbound");
            node.SubItems.Add(ERiftPacketFieldType.Packet.ToString());
            PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == pPacket.Outbound && d.Opcode == pPacket.Opcode);
            if (description != null && description.Name.Trim().Length > 0) node.SubItems.Add(description.Name);
            else node.SubItems.Add("");
            node.SubItems.Add("0x" + pPacket.Opcode.ToString("X8"));
            Parse(node, pPacket);
            mPacketView.Nodes.Add(node);

            mPacketView.EndUpdate();
            mPacketView.AdjustScrollbars();

            if (!pRefreshing) mPlugins.ForEach(p => p.OnPacket(pPacket));
        }

        public sealed class FieldNodeTag
        {
            public RiftPacket Packet;
            public RiftPacketField Field;
            public FieldNodeTag(RiftPacket pPacket, RiftPacketField pField) { Packet = pPacket; Field = pField; }
        }
        public sealed class OpcodeSearchItem
        {
            public bool Outbound;
            public int Opcode;
            public string Name;

            public OpcodeSearchItem(bool pOutbound, int pOpcode, string pName)
            {
                Outbound = pOutbound;
                Opcode = pOpcode;
                Name = pName;
            }
            public override string ToString()
            {
                StringBuilder display = new StringBuilder();
                display.Append(Outbound ? "Outbound  " : "Inbound   ");
                display.Append("0x" + Opcode.ToString("X8"));
                if (Name.Length > 0) display.Append("  " + Name);
                return display.ToString();
            }
        }
        private void Parse(TreeListNode pNode, RiftPacket pPacket)
        {
            bool exists = false;
            foreach (object item in mOpcodeSearchCombo.Items)
            {
                OpcodeSearchItem opcodeItem = item as OpcodeSearchItem;
                exists = opcodeItem.Outbound == pPacket.Outbound && opcodeItem.Opcode == pPacket.Opcode;
                if (exists) break;
            }
            if (!exists)
            {
                PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == pPacket.Outbound && d.Opcode == pPacket.Opcode);
                mOpcodeSearchCombo.Items.Add(new OpcodeSearchItem(pPacket.Outbound, pPacket.Opcode, description != null && description.Name.Trim().Length > 0 ? description.Name : ""));
            }
            foreach (RiftPacketField field in pPacket.Fields)
            {
                TreeListNode fieldNode = new TreeListNode();
                if (field.Type == ERiftPacketFieldType.Packet) fieldNode.Tag = field.Value.Packet;
                else fieldNode.Tag = new FieldNodeTag(pPacket, field);
                fieldNode.Text = "";
                fieldNode.SubItems.Add("");
                fieldNode.SubItems.Add(field.Type.ToString());
                if (field.Type == ERiftPacketFieldType.Packet)
                {
                    PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == field.Value.Packet.Outbound && d.Opcode == field.Value.Packet.Opcode);
                    if (description != null && description.Name.Trim().Length > 0) fieldNode.SubItems.Add(description.Name + " (Field " + field.Index.ToString() + ")");
                    else fieldNode.SubItems.Add("Field " + field.Index.ToString());
                }
                else
                {
                    PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == pPacket.Outbound && d.Opcode == pPacket.Opcode);
                    if (description != null)
                    {
                        PacketDescriptions.PacketDescription.PacketField fieldDescription = description.Fields.Find(f => f.Index == field.Index);
                        if (fieldDescription != null && fieldDescription.Name.Trim().Length > 0) fieldNode.SubItems.Add(fieldDescription.Name + " (Field " + field.Index.ToString() + ")");
                        else fieldNode.SubItems.Add("Field " + field.Index.ToString());
                    }
                    else fieldNode.SubItems.Add("Field " + field.Index.ToString());
                }
                ParseValue(fieldNode, field.Type, field.Value);
                pNode.Nodes.Add(fieldNode);
            }
        }
        private void ParseValue(TreeListNode pNode, ERiftPacketFieldType pType, RiftPacketFieldValue pValue)
        {
            switch (pType)
            {
                case ERiftPacketFieldType.False: pNode.SubItems.Add("False"); break;
                case ERiftPacketFieldType.True: pNode.SubItems.Add("True"); break;
                case ERiftPacketFieldType.Unsigned7BitEncoded: pNode.SubItems.Add(pValue.Long.ToString()); break;
                case ERiftPacketFieldType.Signed7BitEncoded: pNode.SubItems.Add(pValue.Long.ToString()); break;
                case ERiftPacketFieldType.Raw4Bytes: pNode.SubItems.Add(BitConverter.ToString(pValue.Bytes).Replace('-', ' ')); break;
                case ERiftPacketFieldType.Raw8Bytes: pNode.SubItems.Add(BitConverter.ToString(pValue.Bytes).Replace('-', ' ')); break;
                case ERiftPacketFieldType.ByteArray: pNode.SubItems.Add(BitConverter.ToString(pValue.Bytes, 0, Math.Min(8, pValue.Bytes.Length)).Replace('-', ' ') + (pValue.Bytes.Length > 8 ? " ..." : "")); break;
                case ERiftPacketFieldType.Packet:
                    {
                        pNode.SubItems.Add("0x" + pValue.Packet.Opcode.ToString("X8"));
                        Parse(pNode, pValue.Packet);
                        break;
                    }
                case ERiftPacketFieldType.List:
                    {
                        pNode.SubItems.Add("Type = " + pValue.List.Type.ToString());
                        for (int index = 0; index < pValue.List.Count; ++index)
                        {
                            RiftPacketFieldValue value = pValue.List[index];
                            TreeListNode listValueNode = new TreeListNode();
                            listValueNode.Text = "";
                            listValueNode.SubItems.Add("");
                            listValueNode.SubItems.Add(value.Type.ToString());
                            if (value.Type == ERiftPacketFieldType.Packet)
                            {
                                PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == value.Packet.Outbound && d.Opcode == value.Packet.Opcode);
                                if (description != null && description.Name.Trim().Length > 0) listValueNode.SubItems.Add(description.Name + " (Index " + index.ToString() + ")");
                                else listValueNode.SubItems.Add("Index " + index.ToString());
                                listValueNode.Tag = value.Packet;
                            }
                            else
                            {
                                listValueNode.SubItems.Add("Index " + index.ToString());
                                listValueNode.Tag = value;
                            }
                            ParseValue(listValueNode, value.Type, value);
                            pNode.Nodes.Add(listValueNode);
                        }
                        break;
                    }
                case ERiftPacketFieldType.Dictionary:
                    {
                        pNode.SubItems.Add("Key Type = " + pValue.Dictionary.KeyType.ToString() + ", Value Type = " + pValue.Dictionary.ValueType.ToString());
                        for (int index = 0; index < pValue.Dictionary.Count; ++index)
                        {
                            TreeListNode dictionaryKeyValueNode = new TreeListNode();
                            dictionaryKeyValueNode.Text = "";
                            dictionaryKeyValueNode.SubItems.Add("");
                            dictionaryKeyValueNode.SubItems.Add("");
                            dictionaryKeyValueNode.SubItems.Add("Index " + index.ToString());

                            RiftPacketFieldValue keyValue = pValue.Dictionary[index].Key;
                            RiftPacketFieldValue valueValue = pValue.Dictionary[index].Value;
                            dictionaryKeyValueNode.SubItems.Add("");

                            TreeListNode dictionaryKeyNode = new TreeListNode();
                            dictionaryKeyNode.Text = "";
                            dictionaryKeyNode.SubItems.Add("");
                            dictionaryKeyNode.SubItems.Add(keyValue.Type.ToString());
                            if (keyValue.Type == ERiftPacketFieldType.Packet)
                            {
                                PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == keyValue.Packet.Outbound && d.Opcode == keyValue.Packet.Opcode);
                                if (description != null && description.Name.Trim().Length > 0) dictionaryKeyNode.SubItems.Add(description.Name + " (Key)");
                                else dictionaryKeyNode.SubItems.Add("Key");
                                dictionaryKeyNode.Tag = keyValue.Packet;
                            }
                            else
                            {
                                dictionaryKeyNode.SubItems.Add("Key");
                                dictionaryKeyNode.Tag = keyValue;
                            }

                            ParseValue(dictionaryKeyNode, keyValue.Type, keyValue);
                            dictionaryKeyValueNode.Nodes.Add(dictionaryKeyNode);

                            if (valueValue != null)
                            {
                                TreeListNode dictionaryValueNode = new TreeListNode();
                                dictionaryValueNode.Text = "";
                                dictionaryValueNode.SubItems.Add("");
                                dictionaryValueNode.SubItems.Add(valueValue.Type.ToString());
                                if (valueValue.Type == ERiftPacketFieldType.Packet)
                                {
                                    PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == valueValue.Packet.Outbound && d.Opcode == valueValue.Packet.Opcode);
                                    if (description != null && description.Name.Trim().Length > 0) dictionaryValueNode.SubItems.Add(description.Name + " (Value)");
                                    else dictionaryValueNode.SubItems.Add("Value");
                                    dictionaryValueNode.Tag = valueValue.Packet;
                                }
                                else
                                {
                                    dictionaryValueNode.SubItems.Add("Value");
                                    dictionaryValueNode.Tag = valueValue;
                                }
                                ParseValue(dictionaryValueNode, valueValue.Type, valueValue);
                                dictionaryKeyValueNode.Nodes.Add(dictionaryValueNode);
                            }

                            pNode.Nodes.Add(dictionaryKeyValueNode);
                        }
                        break;
                    }
                default: pNode.SubItems.Add(""); break;
            }
        }

        public void OpenFile(string pFilename)
        {
            mFileSaveMenu.Enabled = false;
            mTerminated = true;
            int percent = 0;
            using (FileStream stream = new FileStream(pFilename, FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);
                mLocalPort = 8000;
                while (stream.Position < stream.Length)
                {
                    long timestamp = 0;
                    bool outbound = false;
                    int size = (int)reader.BaseStream.Length;
                    byte[] buffer = reader.ReadBytes(size);
                    RiftPacketReader packetReader = new RiftPacketReader(new DateTime(timestamp), outbound, buffer, 0, buffer.Length);
                    RiftPacket packet;
                    int sizeOfPacket;
                    packetReader.ReadPacket(out packet, out sizeOfPacket);
                    packet.Raw = buffer;
                    AddPacket(packet);

                    int newPercent = (int)((stream.Position * 100) / stream.Length);
                    while (newPercent > percent)
                    {
                        ++percent;
                        ProgressUpdate();
                    }

                }
            }
            Text = string.Format("Port {0} (ReadOnly)", mLocalPort);
        }

        private void mFileSaveMenu_Click(object pSender, EventArgs pArgs)
        {
            if (mFilename == null)
            {
                mSaveDialog.FileName = string.Format("Port {0}", mLocalPort);
                if (mSaveDialog.ShowDialog(this) == DialogResult.OK) mFilename = mSaveDialog.FileName;
                else return;
            }
            using (FileStream stream = new FileStream(mFilename, FileMode.Create, FileAccess.Write))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(mLocalPort);
                foreach (RiftPacket packet in mPackets) packet.Save(writer);
                stream.Flush();
            }
            if (mTerminated)
            {
                mFileSaveMenu.Enabled = false;
                Text = string.Format("Port {0} (ReadOnly)", mLocalPort);
            }
        }

        private void mPacketItemMenu_Opening(object pSender, CancelEventArgs pArgs)
        {
            if (mPacketView.SelectedNodes.Count == 0) pArgs.Cancel = true;
            else
            {
                TreeListNode selectedNode = mPacketView.SelectedNodes[0];
                if (selectedNode.Tag == null) pArgs.Cancel = true;
                else
                {
                    if (selectedNode.Tag is RiftPacket)
                    {
                        RiftPacket packet = selectedNode.Tag as RiftPacket;
                        PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                        if (description != null) mPacketItemNameBox.Text = description.Name;
                        else mPacketItemNameBox.Text = "";
                    }
                    else if (selectedNode.Tag is FieldNodeTag)
                    {
                        RiftPacket packet = (selectedNode.Tag as FieldNodeTag).Packet;
                        RiftPacketField field = (selectedNode.Tag as FieldNodeTag).Field;
                        PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                        if (description != null)
                        {
                            if (field.Type == ERiftPacketFieldType.Packet) mPacketItemNameBox.Text = description.Name;
                            else
                            {
                                PacketDescriptions.PacketDescription.PacketField fieldDescription = description.Fields.Find(f => f.Index == field.Index);
                                if (fieldDescription != null && fieldDescription.Name.Trim().Length > 0) mPacketItemNameBox.Text = fieldDescription.Name;
                                else mPacketItemNameBox.Text = "";
                            }
                        }
                        else mPacketItemNameBox.Text = "";
                    }
                }
            }
        }

        private void mPacketItemMenu_Opened(object pSender, EventArgs pArgs)
        {
            mPacketItemNameBox.Focus();
            mPacketItemNameBox.SelectAll();
        }

        private void mPacketItemNameBox_KeyDown(object pSender, KeyEventArgs pArgs)
        {
            if (pArgs.Modifiers == Keys.None && pArgs.KeyCode == Keys.Enter)
            {
                TreeListNode selectedNode = mPacketView.SelectedNodes[0];
                if (selectedNode.Tag != null)
                {
                    if (selectedNode.Tag is RiftPacket)
                    {
                        RiftPacket packet = selectedNode.Tag as RiftPacket;
                        PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                        if (description == null)
                        {
                            description = new PacketDescriptions.PacketDescription();
                            description.Outbound = packet.Outbound;
                            description.Opcode = packet.Opcode;
                            PacketDescriptions.Instance.Descriptions.Add(description);
                        }
                        description.Name = mPacketItemNameBox.Text;
                        PacketDescriptions.Instance.Save();
                        RefreshPackets();
                    }
                    else if (selectedNode.Tag is FieldNodeTag)
                    {
                        RiftPacket packet = (selectedNode.Tag as FieldNodeTag).Packet;
                        RiftPacketField field = (selectedNode.Tag as FieldNodeTag).Field;
                        PacketDescriptions.PacketDescription description = PacketDescriptions.Instance.Descriptions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                        if (description == null)
                        {
                            description = new PacketDescriptions.PacketDescription();
                            description.Outbound = packet.Outbound;
                            description.Opcode = packet.Opcode;
                            PacketDescriptions.Instance.Descriptions.Add(description);
                        }
                        PacketDescriptions.PacketDescription.PacketField fieldDescription = description.Fields.Find(f => f.Index == field.Index);
                        if (fieldDescription == null)
                        {
                            fieldDescription = new PacketDescriptions.PacketDescription.PacketField();
                            fieldDescription.Index = field.Index;
                            description.Fields.Add(fieldDescription);
                        }
                        fieldDescription.Name = mPacketItemNameBox.Text;
                        PacketDescriptions.Instance.Save();
                        RefreshPackets();
                    }
                }
                pArgs.SuppressKeyPress = true;
                mPacketItemMenu.Close();
            }
        }

        private void RefreshPackets()
        {
            RiftPacket selectedPacket = null;
            int selectedFieldIndex = -1;
            if (mPacketView.SelectedNodes.Count > 0)
            {
                TreeListNode selectedNode = mPacketView.SelectedNodes[0];
                if (selectedNode.Tag != null)
                {
                    if (selectedNode.Tag is RiftPacket) selectedPacket = selectedNode.Tag as RiftPacket;
                    else if (selectedNode.Tag is FieldNodeTag)
                    {
                        selectedPacket = (selectedNode.Tag as FieldNodeTag).Packet;
                        selectedFieldIndex = (selectedNode.Tag as FieldNodeTag).Field.Index;
                    }
                }
            }
            mPacketView.Nodes.Clear();
            mOpcodeSearchCombo.Items.Clear();

            foreach (RiftPacket packet in mPackets)
            {
                AddPacket(packet, true);
            }
            mPacketView.AdjustScrollbars();
        }

        private void mStatusStrip_Resize(object pSender, EventArgs pArgs)
        {
            mStatusProgressBar.Width = mStatusStrip.Width - (mStatusProgressLabel.Width + mStatusProgressStateLabel.Width);
        }

        private void mPacketView_SelectedItemChanged(object pSender, EventArgs pArgs)
        {
            TreeListNode selectedNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
            if (selectedNode == null || selectedNode.Tag == null || !(selectedNode.Tag is FieldNodeTag || selectedNode.Tag is RiftPacketFieldValue)) mHints.SelectedObject = null;
            else
            {
                ERiftPacketFieldType type = ERiftPacketFieldType.False;
                RiftPacketFieldValue value = null;
                if (selectedNode.Tag is FieldNodeTag)
                {
                    type = (selectedNode.Tag as FieldNodeTag).Field.Type;
                    value = (selectedNode.Tag as FieldNodeTag).Field.Value;
                }
                else if (selectedNode.Tag is RiftPacketFieldValue)
                {
                    type = (selectedNode.Tag as RiftPacketFieldValue).Type;
                    value = selectedNode.Tag as RiftPacketFieldValue;
                }
                switch (type)
                {
                    case ERiftPacketFieldType.Unsigned7BitEncoded: mHints.SelectedObject = new HintUnsigned7BitEncoded(value.Long); break;
                    case ERiftPacketFieldType.Signed7BitEncoded: mHints.SelectedObject = new HintSigned7BitEncoded(value.Long); break;
                    case ERiftPacketFieldType.Raw4Bytes: mHints.SelectedObject = new HintRaw4Bytes(value.Bytes); break;
                    case ERiftPacketFieldType.Raw8Bytes: mHints.SelectedObject = new HintRaw8Bytes(value.Bytes); break;
                    case ERiftPacketFieldType.ByteArray: mHints.SelectedObject = new HintByteArray(value.Bytes); break;
                    default: mHints.SelectedObject = null; break;
                }
            }
        }

        private void mOpcodeSearchCombo_SelectedIndexChanged(object pSender, EventArgs pArgs)
        {
            mOpcodeSearchButton.Enabled = mOpcodeSearchCombo.SelectedItem != null;
        }

        private void mOpcodeSearchButton_Click(object pSender, EventArgs pArgs)
        {
            TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
            OpcodeSearchItem searchItem = mOpcodeSearchCombo.SelectedItem as OpcodeSearchItem;

            TreeListNode nextNode = FindNextPacket(mPacketView.Nodes, ref startNode, searchItem.Outbound, searchItem.Opcode);
            if (nextNode != null) mPacketView.SelectNode(nextNode);
            else MessageBox.Show(this, "No more packets found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private TreeListNode FindNextPacket(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, bool pOutbound, int pOpcode)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null && node.Tag is RiftPacket)
                {
                    RiftPacket packet = node.Tag as RiftPacket;
                    if (packet.Outbound == pOutbound && packet.Opcode == pOpcode) return node;
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextPacket(node.Nodes, ref pAfterNode, pOutbound, pOpcode);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private void mFieldSearchBooleanButton_Click(object pSender, EventArgs pArgs)
        {
            TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
            bool searchItem = mFieldSearchBooleanTrueRadio.Checked;
            TreeListNode nextNode = FindNextBoolean(mPacketView.Nodes, ref startNode, searchItem);
            if (nextNode != null) mPacketView.SelectNode(nextNode);
            else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private TreeListNode FindNextBoolean(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, bool pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == (pValue ? ERiftPacketFieldType.True : ERiftPacketFieldType.False)) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == (pValue ? ERiftPacketFieldType.True : ERiftPacketFieldType.False)) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextBoolean(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private void mFieldSearchUnsignedButton_Click(object pSender, EventArgs pArgs)
        {
            TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
            ulong searchItem = (ulong)mFieldSearchUnsignedNumeric.Value;
            TreeListNode nextNode = FindNextUnsigned(mPacketView.Nodes, ref startNode, searchItem);
            if (nextNode != null) mPacketView.SelectNode(nextNode);
            else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private TreeListNode FindNextUnsigned(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, ulong pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == ERiftPacketFieldType.Unsigned7BitEncoded && (ulong)field.Value.Long == pValue) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == ERiftPacketFieldType.Unsigned7BitEncoded && (ulong)value.Long == pValue) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextUnsigned(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private void mFieldSearchSignedButton_Click(object pSender, EventArgs pArgs)
        {
            TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
            long searchItem = (long)mFieldSearchSignedNumeric.Value;
            TreeListNode nextNode = FindNextSigned(mPacketView.Nodes, ref startNode, searchItem);
            if (nextNode != null) mPacketView.SelectNode(nextNode);
            else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private TreeListNode FindNextSigned(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, long pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == ERiftPacketFieldType.Signed7BitEncoded && field.Value.Long == pValue) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == ERiftPacketFieldType.Signed7BitEncoded && value.Long == pValue) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextSigned(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private static bool CompareByteArrays(byte[] pArray1, byte[] pArray2)
        {
            if (pArray1.Length != pArray2.Length) return false;
            for (int index = 0; index < pArray1.Length; ++index) if (pArray1[index] != pArray2[index]) return false;
            return true;
        }
        private static bool CompareByteArrays(byte[] pArray1, int pArray1Start, byte[] pArray2, int pArray2Start, int pLength)
        {
            if (pArray1 == null || pArray2 == null) return false;
            if (pArray1.Length - pArray1Start < pLength) return false;
            if (pArray2.Length - pArray2Start < pLength) return false;
            for (int count = 0; count < pLength; ++count) if (pArray1[pArray1Start + count] != pArray2[pArray2Start + count]) return false;
            return true;
        }

        private void mFieldSearchRaw4BytesButton_Click(object pSender, EventArgs pArgs)
        {
            bool invalid = false;
            float floatValue = 0.0f;
            uint uintValue = 0;
            int intValue = 0;
            byte[] searchItem = null;
            if (mFieldSearchRaw4BytesBox.Text.Length == 0) invalid = true;
            else if (uint.TryParse(mFieldSearchRaw4BytesBox.Text, out uintValue)) searchItem = BitConverter.GetBytes(uintValue);
            else if (int.TryParse(mFieldSearchRaw4BytesBox.Text, out intValue)) searchItem = BitConverter.GetBytes(intValue);
            else if (float.TryParse(mFieldSearchRaw4BytesBox.Text, out floatValue)) searchItem = BitConverter.GetBytes(floatValue);
            else invalid = true;

            if (invalid) MessageBox.Show(this, "Invalid search parameters, please enter a float, uint, or int.", "Invalid Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
                TreeListNode nextNode = FindNextRaw4Bytes(mPacketView.Nodes, ref startNode, searchItem);
                if (nextNode != null) mPacketView.SelectNode(nextNode);
                else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private TreeListNode FindNextRaw4Bytes(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, byte[] pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == ERiftPacketFieldType.Raw4Bytes && CompareByteArrays(pValue, field.Value.Bytes)) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == ERiftPacketFieldType.Raw4Bytes && CompareByteArrays(pValue, value.Bytes)) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextRaw4Bytes(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private void mFieldSearchRaw8BytesButton_Click(object pSender, EventArgs pArgs)
        {
            bool invalid = false;
            double doubleValue = 0.0f;
            ulong ulongValue = 0;
            long longValue = 0;
            byte[] searchItem = null;
            if (mFieldSearchRaw8BytesBox.Text.Length == 0) invalid = true;
            else if (ulong.TryParse(mFieldSearchRaw8BytesBox.Text, out ulongValue)) searchItem = BitConverter.GetBytes(ulongValue);
            else if (long.TryParse(mFieldSearchRaw8BytesBox.Text, out longValue)) searchItem = BitConverter.GetBytes(longValue);
            else if (double.TryParse(mFieldSearchRaw8BytesBox.Text, out doubleValue)) searchItem = BitConverter.GetBytes(doubleValue);
            else invalid = true;

            if (invalid) MessageBox.Show(this, "Invalid search parameters, please enter a double, ulong, or long.", "Invalid Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
                TreeListNode nextNode = FindNextRaw8Bytes(mPacketView.Nodes, ref startNode, searchItem);
                if (nextNode != null) mPacketView.SelectNode(nextNode);
                else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private TreeListNode FindNextRaw8Bytes(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, byte[] pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == ERiftPacketFieldType.Raw8Bytes && CompareByteArrays(pValue, field.Value.Bytes)) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == ERiftPacketFieldType.Raw8Bytes && CompareByteArrays(pValue, value.Bytes)) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextRaw8Bytes(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }

        private static bool IsValidHexCharacter(char pChar)
        {
            return pChar == '0' || pChar == '1' || pChar == '2' || pChar == '3' || pChar == '4' ||
                   pChar == '5' || pChar == '6' || pChar == '7' || pChar == '8' || pChar == '9' ||
                   pChar == 'a' || pChar == 'A' ||
                   pChar == 'b' || pChar == 'B' ||
                   pChar == 'c' || pChar == 'C' ||
                   pChar == 'd' || pChar == 'D' ||
                   pChar == 'e' || pChar == 'E' ||
                   pChar == 'f' || pChar == 'F';
        }
        private static bool TryStringToByteArray(string pHex, out byte[] pArray)
        {
            pHex = pHex.Replace(" ", "").Replace("-", "");
            pArray = null;
            if (pHex.Length % 2 != 0) return false;
            int totalCharacters = pHex.Length;
            for (int index = 0; index < totalCharacters; ++index) if (!IsValidHexCharacter(pHex[index])) return false;
            pArray = new byte[totalCharacters / 2];
            for (int index = 0; index < totalCharacters; index += 2) pArray[index / 2] = Convert.ToByte(pHex.Substring(index, 2), 16);
            return true;
        }
        private void mFieldSearchByteArrayButton_Click(object pSender, EventArgs pArgs)
        {
            bool invalid = false;
            byte[] searchItem = null;
            if (mFieldSearchByteArrayBox.Text.Length == 0) invalid = true;
            else if (!TryStringToByteArray(mFieldSearchByteArrayBox.Text, out searchItem)) searchItem = Encoding.ASCII.GetBytes(mFieldSearchByteArrayBox.Text);

            if (invalid) MessageBox.Show(this, "Invalid search parameters, please enter a hex string or ascii string.", "Invalid Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                TreeListNode startNode = mPacketView.SelectedNodes.Count == 0 ? null : mPacketView.SelectedNodes[0];
                TreeListNode nextNode = FindNextByteArray(mPacketView.Nodes, ref startNode, searchItem);
                if (nextNode != null) mPacketView.SelectNode(nextNode);
                else MessageBox.Show(this, "No more fields found.", "End Of Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private TreeListNode FindNextByteArray(TreeListNodeCollection pNodes, ref TreeListNode pAfterNode, byte[] pValue)
        {
            bool foundStart = pAfterNode == null;
            foreach (TreeListNode node in pNodes)
            {
                if (foundStart && node.Tag != null)
                {
                    if (node.Tag is FieldNodeTag)
                    {
                        RiftPacketField field = (node.Tag as FieldNodeTag).Field;
                        if (field.Type == ERiftPacketFieldType.ByteArray && CompareByteArrays(pValue, field.Value.Bytes)) return node;
                    }
                    else if (node.Tag is RiftPacketFieldValue)
                    {
                        RiftPacketFieldValue value = node.Tag as RiftPacketFieldValue;
                        if (value.Type == ERiftPacketFieldType.ByteArray && CompareByteArrays(pValue, value.Bytes)) return node;
                    }
                }
                if (!foundStart && node == pAfterNode)
                {
                    foundStart = true;
                    pAfterNode = null;
                }
                TreeListNode found = FindNextByteArray(node.Nodes, ref pAfterNode, pValue);
                if (!foundStart) foundStart = pAfterNode == null;
                if (found != null) return found;
            }
            return null;
        }
    }
}
