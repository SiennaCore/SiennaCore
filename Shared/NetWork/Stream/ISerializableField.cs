using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.NetWork;

namespace Shared
{
    public enum EPacketFieldType
    {
        False = 0,
        True = 1,
        Unsigned7BitEncoded = 2,
        Signed7BitEncoded = 3,
        Raw4Bytes = 4,
        Raw8Bytes = 5,
        ByteArray = 6,
        Invalid = 7,
        Terminator = 8,
        Entry = 9,
        Packet = 10,
        List = 11,
        Dictionary = 12
    }

    public abstract class ISerializableFieldAttribute : Attribute
    {
        public int Index = 0;

        public ISerializableFieldAttribute(int Index)
        {
            this.Index = Index;
        }

        public abstract Type GetSerializableType();
    }

    public abstract class ISerializableField
    {
        public EPacketFieldType PacketType;

        protected object val;

        public object value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
            }
        }

        public abstract void Deserialize(ref PacketInStream Data, Type Field);

        public abstract void Serialize(ref PacketOutStream Data, Type Field);
        public void WriteType(ref PacketOutStream Data, int Index)
        {
            long FieldType;
            PacketOutStream.Encode2Parameters(out FieldType, (int)PacketType, Index);
            Data.WriteEncoded7Bit(FieldType);
        }
    }
}
