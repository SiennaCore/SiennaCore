using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Shared.NetWork;

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
        public override void Deserialize(ref PacketInStream Data)
        {
            long Size = Data.ReadEncoded7Bit();
            val = Data.Read((int)Size);
        }

        public override bool Serialize(ref PacketOutStream Data)
        {
            byte[] Result = new byte[0];

            if (val is string)
            {
                Result = UTF8Encoding.UTF8.GetBytes((val as string));
            }
            else if (val is byte[])
            {
                Result = (val as byte[]);
            }

            if (Result == null || Result.Length <= 0)
                return false;

            Data.WriteEncoded7Bit(Result.Length);
            Data.Write(Result);

            return true;
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            if (Field.Equals(typeof(byte[])))
                Info.SetValue(Packet, val);
            else if(Field.Equals(typeof(string)))
                Info.SetValue(Packet,Marshal.ConvertToString((byte[])val));
        }
    }
}