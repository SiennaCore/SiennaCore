using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RiftShark
{
    public sealed class RiftPacket
    {
        public readonly DateTime Timestamp;
        public readonly bool Outbound;
        public readonly int Opcode;
        public byte[] Raw = null;
        private List<RiftPacketField> mFields = new List<RiftPacketField>();

        public RiftPacket(DateTime pTimestamp, bool pOutbound, int pOpcode)
        {
            Timestamp = pTimestamp;
            Outbound = pOutbound;
            Opcode = pOpcode;
        }

        public List<RiftPacketField> Fields { get { return mFields; } }

        public bool GetFieldByIndex(out RiftPacketField pField, int pIndex)
        {
            pField = mFields.Find(f => f.Index == pIndex);
            return pField != null;
        }

        internal void AddBooleanField(int pIndex, bool pValue)
        {
            RiftPacketField field = new RiftPacketField(pValue ? ERiftPacketFieldType.True : ERiftPacketFieldType.False, pIndex);
            mFields.Add(field);
        }
        internal void AddUnsigned7BitEncodedField(int pIndex, long pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Unsigned7BitEncoded, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Unsigned7BitEncoded, pValue);
            mFields.Add(field);
        }
        internal void AddSigned7BitEncodedField(int pIndex, long pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Signed7BitEncoded, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Signed7BitEncoded, pValue);
            mFields.Add(field);
        }
        internal void AddRaw4BytesField(int pIndex, byte[] pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Raw4Bytes, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Raw4Bytes, pValue);
            mFields.Add(field);
        }
        internal void AddRaw8BytesField(int pIndex, byte[] pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Raw8Bytes, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Raw8Bytes, pValue);
            mFields.Add(field);
        }
        internal void AddByteArrayField(int pIndex, byte[] pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.ByteArray, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.ByteArray, pValue);
            mFields.Add(field);
        }
        internal void AddPacketField(int pIndex, RiftPacket pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Packet, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Packet, pValue);
            mFields.Add(field);
        }
        internal void AddListField(int pIndex, RiftPacketFieldValueList pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.List, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.List, pValue);
            mFields.Add(field);
        }
        internal void AddDictionaryField(int pIndex, RiftPacketFieldValueDictionary pValue)
        {
            RiftPacketField field = new RiftPacketField(ERiftPacketFieldType.Dictionary, pIndex);
            field.Value = new RiftPacketFieldValue(ERiftPacketFieldType.Dictionary, pValue);
            mFields.Add(field);
        }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Timestamp.Ticks);
            pWriter.Write(Outbound);
            pWriter.Write(Raw.Length);
            pWriter.Write(Raw);
        }
    }
}
