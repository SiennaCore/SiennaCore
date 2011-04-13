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
            if (val is string)
            {
                Data.WriteEncoded7Bit((val as string).Length);
                Data.WriteStringBytes((val as string));
            }
            else if (val is byte[])
            {
                Data.WriteEncoded7Bit((val as byte[]).Length);
                Data.Write((val as byte[]));
            }
            else
                return false;

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