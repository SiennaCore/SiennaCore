using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class ListBitAttribute : ISerializableFieldAttribute
    {
        public ListBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(ListBitField);
        }
    }

    public class ListBitField : ISerializableField
    {
        public ListBitField()
        {
            PacketType = EPacketFieldType.List;
        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {
            long ListData = Data.ReadEncoded7Bit();

            int ListType;
            int ListCount;
            PacketInStream.Decode2Parameters(ListData, out ListType, out ListCount);
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            if (Field.Equals(typeof(List<ISerializablePacket>)))
            {
                List<ISerializablePacket> Elements = val as List<ISerializablePacket>;
                
                long ListData;
                
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Packet, 0);
                Data.WriteEncoded7Bit(ListData);

                foreach (ISerializablePacket Packet in Elements)
                    PacketProcessor.WritePacket(ref Data, Packet.GetType(), Packet, false);
            }
        }
    }
}
