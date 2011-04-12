using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class PacketBitAttribute : ISerializableFieldAttribute
    {
        public PacketBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(PacketBitField);
        }
    }

    public class PacketBitField : ISerializableField
    {
        public PacketBitField()
        {
            PacketType = EPacketFieldType.Packet;
        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {
            val = PacketProcessor.ProcessGameDataStream(ref Data);
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            PacketProcessor.WritePacket(ref Data, val.GetType(), val as ISerializablePacket);
        }
    }
}
