using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class Unsigned7BitAttribute : ISerializableFieldAttribute
    {
        public Unsigned7BitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(Unsigned7BitField);
        }
    }

    public class Unsigned7BitField : ISerializableField
    {
        public Unsigned7BitField()
        {
            PacketType = EPacketFieldType.Unsigned7BitEncoded;
        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {
            val = Data.ReadEncoded7Bit();
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            Data.WriteEncoded7Bit((long)val);
        }
    }
}
