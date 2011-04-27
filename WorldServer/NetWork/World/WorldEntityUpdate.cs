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
        public long Unk = 1000;

        [BoolBit(5)]
        public bool Fixed1 = true;

        [BoolBit(7)]
        public bool Fixed2 = true;

        public void Build()
        {
            /*Packets = new List<ISerializablePacket>();

            // Unk Packet
            ISerializablePacket UnkPck = new ISerializablePacket();
            UnkPck.Opcode = 0x0259;
            UnkPck.AddField(3, EPacketFieldType.Raw4Bytes, new byte[4] { 0x00, 0x00, 0x48, 0x42 });

            // World Position
            WorldPositionExtra WorldPos = new WorldPositionExtra();
            WorldPos.MapName = "guardian_map";

            ISerializablePacket UnkPck3 = new ISerializablePacket();
            UnkPck3.Opcode = 0x026B;

            Packets.Add(UnkPck);
            Packets.Add(CreateJunk(0x025A));
            Packets.Add(WorldPos);*/

            // Dic
            Packets.Add(CreateJunk(0x026B));

            Packets.Add(CreateJunk(0x026E));
            Packets.Add(CreateJunk(0x0272));
            Packets.Add(CreateJunk(0x0289));
            Packets.Add(CreateJunk(0x028A));

            // 28F
            Packets.Add(CreateJunk(0x028F));
            Packets.Add(CreateJunk(0x0294));
            Packets.Add(CreateJunk(0x02AE));
            Packets.Add(CreateJunk(0x033A));
            Packets.Add(CreateJunk(0x08A2));
  
            // Others

            Packets.Add(CreateJunk(0x08FC));

            // Others2

            Packets.Add(CreateJunk(0x0F41));
            Packets.Add(CreateJunk(0x0F45));
        }

        private ISerializablePacket CreateJunk(long Opcode)
        {
            ISerializablePacket Junk = new ISerializablePacket();
            Junk.Opcode = Opcode;
            return Junk;
        }
    }
}
