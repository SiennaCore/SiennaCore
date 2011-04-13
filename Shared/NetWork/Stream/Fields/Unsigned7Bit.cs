using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
        public override void Deserialize(ref PacketInStream Data)
        {
            val = Data.ReadEncoded7Bit();
        }

        public override void Serialize(ref PacketOutStream Data)
        {
            Data.WriteEncoded7Bit((long)val);
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            Info.SetValue(Packet, (long)val);
        }
    }
}
