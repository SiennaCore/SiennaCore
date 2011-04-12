using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class Raw8BitAttribute : ISerializableFieldAttribute
    {
        public Raw8BitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(Raw8BitBitField);
        }
    }

    public class Raw8BitBitField : ISerializableField
    {
        public Raw8BitBitField()
        {
            PacketType = EPacketFieldType.Raw8Bytes;
        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {
            if (Field.Equals(typeof(UInt64)) || Field.Equals(typeof(Int64)))
                val = Data.GetUint64R();
            else
                val = Data.Read(8);
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            if (Field.Equals(typeof(UInt64)) || Field.Equals(typeof(Int64)))
                Data.WriteUInt64R((uint)val);
            else if (Field.Equals(typeof(byte[])))
                Data.Write((byte[])val);
        }
    }
}
