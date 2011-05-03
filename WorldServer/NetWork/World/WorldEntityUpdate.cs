using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldEntityUpdate)]
    public class WorldEntityUpdate : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;

        [ListBit(1)]
        public List<ISerializablePacket> Packets;

        [Unsigned7Bit(4)]
        public long Unk = 1001;

        [BoolBit(5)]
        public bool Fixed1 = true;

        [BoolBit(7)]
        public bool Fixed2 = true;

        protected ISerializablePacket GetFieldByOpcode(long Opcode)
        {
            foreach (ISerializablePacket ipck in Packets)
                if (ipck.Opcode == Opcode)
                    return ipck;

            return null;
        }

        public void Build(Character Char)
        {
            GUID = Char.Id;

            if(Packets == null)
                Packets = new List<ISerializablePacket>();

            // Unk Packet
            ISerializablePacket UnkPck = new ISerializablePacket();
            UnkPck.Opcode = 0x0259;
            UnkPck.AddField(3, EPacketFieldType.Raw4Bytes, (float)50.0f);

            // World Position
            ISerializablePacket posextra = GetFieldByOpcode((long)Opcodes.WorldPositionExtra);
            UpdateClientPosition(Char, ref posextra);

            ISerializablePacket UnkPck3 = new ISerializablePacket();
            UnkPck3.Opcode = 0x026B;

            Packets.Add(UnkPck);
            Packets.Add(CreateJunk(0x025A));

            // Dic
            //Packets.Add(CreateJunk(0x026B));

            Packets.Add(CreateJunk(0x026E));

            // Unk Data, Social Related
            /*ISerializablePacket UnkPck4 = new ISerializablePacket();
            UnkPck4.Opcode = 0x0272;
            ISerializablePacket UnkPck4Pck = new ISerializablePacket();
            UnkPck4Pck.Opcode = 0x0CC;
            UnkPck4Pck.AddField(3, EPacketFieldType.Unsigned7BitEncoded, (long)907);
            UnkPck4Pck.AddField(5, EPacketFieldType.Unsigned7BitEncoded, (long)1501);
            UnkPck4.AddField(1, EPacketFieldType.Packet, UnkPck4Pck);
            Packets.Add(UnkPck4);*/
            Packets.Add(CreateJunk(0x0272));

            // Friend List ?
            Packets.Add(CreateJunk(0x0289));

            // Guild ?
            Packets.Add(CreateJunk(0x028A));

            // Another Social Junk ?
            ISerializablePacket UnkPck5 = new ISerializablePacket();
            UnkPck5.Opcode = 0x028F;
            ISerializablePacket UnkPck5Pck = new ISerializablePacket();
            UnkPck5Pck.Opcode = 0x0354;
            UnkPck5Pck.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)2);
            UnkPck5.AddField(4, EPacketFieldType.Packet, UnkPck5Pck);
            Packets.Add(UnkPck5);
            
            ISerializablePacket CharInfo = GetFieldByOpcode(0x02AE);
            ISerializablePacket CharStats = GetFieldByOpcode(0x0294);

            PrepareCharInfo(Char, ref CharInfo);
            PrepareStatsInfo(Char, ref CharStats);

            /*Packets.Add(CreateJunk(0x033A));
            Packets.Add(CreateJunk(0x08A2));*/
  
            // Others

            Packets.Add(CreateJunk(0x08FC));

            // Others2

            /*Packets.Add(CreateJunk(0x0F41));
            Packets.Add(CreateJunk(0x0F45));*/

            // 0x1A20 Macro Related
        }

        protected void UpdateClientPosition(Character Char, ref ISerializablePacket pck)
        {
            WorldPositionExtra WorldPos = pck as WorldPositionExtra;
            WorldPos.MapId = 676;
            WorldPos.Position = new List<float> { 1113.03967f, 920.1114f, 1444.58533f }; // X,Y,Z
            WorldPos.MapName = "guardian_map";
        }

        // 0x294 = STATS_DEFINITION ?
        protected void PrepareStatsInfo(Character Char, ref ISerializablePacket pck)
        {
            // CHAR_CLASS ?
            //pck.AddField(2, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Class);    

            Items_Template[] Templates = CharacterMgr.Instance.GetEquipedItems(Char.Id);

            Dictionary<long, ISerializablePacket> CharDesc = new Dictionary<long, ISerializablePacket>();

            foreach (Items_Template Template in Templates)
            {
                Model_Info model = CharacterMgr.Instance.GetItemModel(Template.ModelEntry, Char.Race, Char.Sex);

                CharacterDesc Desc = new CharacterDesc();

                Desc.AddField(4, EPacketFieldType.Raw4Bytes, (uint)model.Field_4);

                if (model.Field_5 != 0)
                    Desc.AddField(5, EPacketFieldType.Raw4Bytes, model.Field_5);

                if (model.Field_6 != 0)
                    Desc.AddField(6, EPacketFieldType.Raw4Bytes, model.Field_6);

                if (Template.Slot == 10)
                    Desc.AddField(7, EPacketFieldType.Unsigned7BitEncoded, (long)model.Field_7);
                else
                    Desc.AddField(7, EPacketFieldType.Raw4Bytes, (uint)model.Field_7);

                Desc.Field8 = new CharacterInfoCache();
                Desc.Field8.CacheIdentifier = (uint)model.CacheID;

                CharDesc.Add((long)Template.Slot, Desc);
            }

            Model_Info HairEntry = CharacterMgr.Instance.GetModelForCacheID(Char.HairModelID);

            if (HairEntry != null)
            {
                CharacterDesc DescHair = new CharacterDesc();

                DescHair.AddField(4, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_4);
                DescHair.AddField(7, EPacketFieldType.Raw4Bytes, (uint)HairEntry.Field_7);

                DescHair.Field8 = new CharacterInfoCache();
                DescHair.Field8.CacheIdentifier = (uint)HairEntry.CacheID;

                CharDesc.Add((long)46, DescHair);
            }

            pck.AddField(11, EPacketFieldType.Dictionary, CharDesc);
        }

        // 0x2AE = CHARACTER_DEFINITION ?
        protected void PrepareCharInfo(Character Char, ref ISerializablePacket pck)
        {
            pck.AddField(12, EPacketFieldType.ByteArray, Char.Name);

            Realm realm_data = CharacterMgr.Instance.GetRealm((byte)Char.RealmId);
            realm_data.GenerateName();

            pck.AddField(54, EPacketFieldType.Unsigned7BitEncoded, realm_data.RiftId);

            byte[] CustomData = Shared.NetWork.Marshal.StringToUTF8ByteArray(Char.Data);
            PacketInStream CustomStream = new PacketInStream(CustomData, CustomData.Length);
            pck.AddField(56, EPacketFieldType.Packet, PacketProcessor.ReadPacket(ref CustomStream) as CharacterCustom);

            // Field 60
            // Character Specializations            
        }

        private ISerializablePacket CreateJunk(long Opcode)
        {
            ISerializablePacket Junk = new ISerializablePacket();
            Junk.Opcode = Opcode;
            return Junk;
        }
    }
}
