using System;

namespace RiftShark
{
    public sealed class RiftPacketField
    {
        public readonly ERiftPacketFieldType Type;
        public readonly int Index;
        public RiftPacketFieldValue Value = null;

        public RiftPacketField(ERiftPacketFieldType pType, int pIndex)
        {
            Type = pType;
            Index = pIndex;
        }

        public ulong ToUInt64()
        {
            if (Type == ERiftPacketFieldType.False) return 0;
            if (Type == ERiftPacketFieldType.True) return 1;
            if (Type == ERiftPacketFieldType.Unsigned7BitEncoded) return (ulong)Value.Long;
            if (Type == ERiftPacketFieldType.Signed7BitEncoded) return (ulong)Value.Long;
            if (Type == ERiftPacketFieldType.Raw4Bytes) return BitConverter.ToUInt32(Value.Bytes, 0);
            if (Type == ERiftPacketFieldType.Raw8Bytes) return BitConverter.ToUInt64(Value.Bytes, 0);
            throw new InvalidCastException();
        }
    }
}
