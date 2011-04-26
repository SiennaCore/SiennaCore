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
            Packets = new List<ISerializablePacket>();

            ISerializablePacket UnkPck = new ISerializablePacket();
            UnkPck.Opcode = 0x0259;
            UnkPck.AddField(3, EPacketFieldType.Raw4Bytes, new byte[4] { 0x00, 0x00, 0x48, 0x42 });

            ISerializablePacket UnkPck2 = new ISerializablePacket();
            UnkPck2.Opcode = 0x025A;

            WorldPositionExtra WorldPos = new WorldPositionExtra();
            WorldPos.MapName = "guardian_map";

            Packets.Add(UnkPck);
            Packets.Add(UnkPck2);
            Packets.Add(WorldPos);
        }
    }
}
