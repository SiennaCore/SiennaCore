using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class Encoded7BitAttribute : ISerializableFieldAttribute
    {
        public Encoded7BitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(Encoded7BitField);
        }
    }

    public class Encoded7BitField : ISerializableField
    {
        public Encoded7BitField()
        {
            PacketType = EPacketFieldType.Signed7BitEncoded;
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
