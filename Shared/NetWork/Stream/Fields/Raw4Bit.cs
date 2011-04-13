using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Shared.NetWork;

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
            return typeof(Raw4BitField);
        }
    }

    public class Raw4BitField : ISerializableField
    {
        public override void Deserialize(ref PacketInStream Data)
        {
            val = Data.Read(4);
        }

        public override void Serialize(ref PacketOutStream Data)
        {
            if (val is UInt32)
                Data.WriteUInt32R((UInt32)val);
            else if (val is Int32)
                Data.WriteInt32((Int32)val);
            else
                Data.Write((byte[])val);
        }

        public override void ApplyToFieldInfo(FieldInfo Info, ISerializablePacket Packet, Type Field)
        {
            byte[] Data = val as byte[];
            object Result = Data;

            if(Field.Equals(typeof(UInt32)))
                Result = Marshal.ConvertToUInt32(Data[3], Data[2], Data[1], Data[0]);
            else if(Field.Equals(typeof(Int32)))
                Result = BitConverter.ToInt32(Data, 0);

            Info.SetValue(Packet, Result);
        }
    }
}
