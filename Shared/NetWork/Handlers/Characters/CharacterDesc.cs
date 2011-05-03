using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.CharacterInfoCache)]
    public class CharacterInfoCache : ISerializablePacket
    {
        [Raw4Bit(0)]
        public uint CacheIdentifier;
    }

    [ISerializableAttribute((long)Opcodes.CharacterInfoDesc)]
    public class CharacterDesc : ISerializablePacket
    {
        [PacketBit(8)]
        public CharacterInfoCache Field8;
    }
}
