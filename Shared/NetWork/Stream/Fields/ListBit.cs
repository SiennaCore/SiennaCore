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


            List<ISerializableField> Fields = new List<ISerializableField>();

            if(Field.Equals(typeof(List<UInt32>)))
            {
                for (int i = 0; i < ListCount; ++i)
                    Fields.Add(PacketProcessor.ReadField(ref Data, (EPacketFieldType)ListType, typeof(UInt32)));

                List<UInt32> Uints = new List<uint>();
                foreach (ISerializableField Fd in Fields)
                    Uints.Add((UInt32)Fd.value);
                val = Uints;
            }
            else if (Field.Equals(typeof(List<long>)))
            {
                for (int i = 0; i < ListCount; ++i)
                    Fields.Add(PacketProcessor.ReadField(ref Data, (EPacketFieldType)ListType, typeof(long)));

                List<long> longs = new List<long>();
                foreach (ISerializableField Fd in Fields)
                    longs.Add((long)Fd.value);
                val = longs;
            }
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            if (Field.Equals(typeof(List<ISerializablePacket>)))
            {
                List<ISerializablePacket> Elements = val as List<ISerializablePacket>;

                long ListData;
                PacketOutStream.Encode2Parameters(out ListData, (int)EPacketFieldType.Packet, Elements.Count);
                Data.WriteEncoded7Bit(ListData);

                int Count = 1;
                foreach (ISerializablePacket Packet in Elements)
                {
                    PacketOutStream NewPacket = new PacketOutStream();
                    PacketProcessor.WritePacket(ref NewPacket, Packet.GetType(), Packet, false);

                    Log.Dump("NewPacket", NewPacket.ToArray(), 0, (int)NewPacket.Length);
                    Data.Write(NewPacket.ToArray());

                    Count++;
                }
            }
        }
    }
}
