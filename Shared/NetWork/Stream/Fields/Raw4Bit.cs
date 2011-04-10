using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class Raw4BitAttribute : ISerializableFieldAttribute
    {
        public Raw4BitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(Raw4BitBitField);
        }
    }

    public class Raw4BitBitField : ISerializableField
    {
        public Raw4BitBitField()
        {
            PacketType = EPacketFieldType.Raw4Bytes;
        }

        public override void Deserialize(ref PacketInStream Data,Type Field)
        {
            if (Field.Equals(typeof(Int32)))
                val = Data.GetInt32R();
            else if (Field.Equals(typeof(UInt32)))
                val = Data.GetUint32R();
            else if (Field.Equals(typeof(float)))
                val = Data.GetFloat();
            else if (Field.Equals(typeof(byte[])))
                val = Data.Read(4);
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            if (Field.Equals(typeof(Int32)))
                Data.WriteInt32R((int)val);
            else if (Field.Equals(typeof(UInt32)))
                Data.WriteUInt32R((uint)val);
            else if (Field.Equals(typeof(float)))
                Data.WriteFloat((float)val);
            else if (Field.Equals(typeof(byte[])))
                Data.Write((byte[])val);
        }
    }
}
