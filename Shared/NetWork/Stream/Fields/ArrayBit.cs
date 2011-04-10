using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class ArrayBitAttribute : ISerializableFieldAttribute
    {
        public ArrayBitAttribute(int Index)
            : base(Index)
        {

        }

        public override Type GetSerializableType()
        {
            return typeof(ArrayBitField);
        }
    }

    public class ArrayBitField : ISerializableField
    {
        public ArrayBitField()
        {
            PacketType = EPacketFieldType.ByteArray;
        }

        public override void Deserialize(ref PacketInStream Data, Type Field)
        {
            long Size = Data.ReadEncoded7Bit();

            if(Field.Equals(typeof(string)))
                val = Data.GetString((int)Size);
            else
                val = Data.Read((int)Size);
        }

        public override void Serialize(ref PacketOutStream Data, Type Field)
        {
            if (Field.Equals(typeof(string)))
            {
                Data.WriteEncoded7Bit((val as string).Length);
                Data.WriteStringBytes((val as string));
            }
            else if(Field.Equals(typeof(byte[])))
            {
                Data.WriteEncoded7Bit((val as byte[]).Length);
                Data.Write((byte[])val);
            }
        }
    }
}