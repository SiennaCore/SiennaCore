using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.TemplateCreationUnknown1Data)]
    public class TemplateCreationUnknown1Data : ISerializablePacket
    {
        [BoolBit(0)]
        public bool Field0;
    }

    [ISerializableAttribute((long)Opcodes.TemplateCreationData)]
    public class TemplateCreationData : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Name;

        [ArrayBit(1)]
        public string Description;

        [Unsigned7Bit(3)]
        public long Field3;

        [Raw4Bit(4)]
        public uint Field4;

        [ListBit(7)]
        public List<long> Field7;

        [PacketBit(9)]
        public ISerializablePacket Field9;

        [Unsigned7Bit(14)]
        public long Field14;

        [BoolBit(19)]
        public bool Field19;

        [Unsigned7Bit(20)]
        public long Field20;

        [Unsigned7Bit(21)]
        public long Field21;

        [ArrayBit(22)]
        public uint Field22;

        [BoolBit(34)]
        public bool Field34;

        [ListBit(37)]
        public List<ISerializablePacket> Field37;

        [PacketBit(40)]
        public ISerializablePacket Field40;

        [BoolBit(51)]
        public bool Field51;

        [BoolBit(75)]
        public bool Field75;
    }

    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreationCacheResponse)]
    public class CacheUpdate : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long CacheType;

        [Raw4Bit(1)]
        public uint Identifier;

        [ListBit(2)]
        public List<ISerializablePacket> CacheData;
    }
}
