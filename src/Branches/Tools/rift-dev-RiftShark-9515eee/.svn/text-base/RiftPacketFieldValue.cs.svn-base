using System;
using System.Collections.Generic;

namespace RiftShark
{
    public sealed class RiftPacketFieldValue
    {
        public readonly ERiftPacketFieldType Type;
        public readonly long Long;
        public readonly byte[] Bytes;
        public readonly RiftPacket Packet;
        public readonly RiftPacketFieldValueList List;
        public readonly RiftPacketFieldValueDictionary Dictionary;

        public RiftPacketFieldValue(ERiftPacketFieldType pType, long pLong)
        {
            Type = pType;
            Long = pLong;
        }
        public RiftPacketFieldValue(ERiftPacketFieldType pType, byte[] pBytes)
        {
            Type = pType;
            Bytes = pBytes;
        }
        public RiftPacketFieldValue(ERiftPacketFieldType pType, RiftPacket pPacket)
        {
            Type = pType;
            Packet = pPacket;
        }
        public RiftPacketFieldValue(ERiftPacketFieldType pType, RiftPacketFieldValueList pList)
        {
            Type = pType;
            List = pList;
        }
        public RiftPacketFieldValue(ERiftPacketFieldType pType, RiftPacketFieldValueDictionary pDictionary)
        {
            Type = pType;
            Dictionary = pDictionary;
        }
    }
    public sealed class RiftPacketFieldValueList : List<RiftPacketFieldValue>
    {
        public readonly ERiftPacketFieldType Type;
        public RiftPacketFieldValueList(ERiftPacketFieldType pType) : base() { Type = pType; }
    }
    public sealed class RiftPacketFieldValueDictionary : List<KeyValuePair<RiftPacketFieldValue, RiftPacketFieldValue>>
    {
        public readonly ERiftPacketFieldType KeyType;
        public readonly ERiftPacketFieldType ValueType;
        public RiftPacketFieldValueDictionary(ERiftPacketFieldType pKeyType, ERiftPacketFieldType pValueType) : base() { KeyType = pKeyType; ValueType = pValueType; }
    }
}
