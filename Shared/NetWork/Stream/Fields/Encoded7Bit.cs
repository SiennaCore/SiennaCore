using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
        public override void Deserialize(ref PacketInStream Data)
        {
            val = Data.ReadEncoded7Bit();
        }

        public override bool Serialize(ref PacketOutStream Data)
        {
            Data.WriteEncoded7Bit((long)val);
            return true;
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            Info.SetValue(Packet, (long)val);
        }
    }
}
