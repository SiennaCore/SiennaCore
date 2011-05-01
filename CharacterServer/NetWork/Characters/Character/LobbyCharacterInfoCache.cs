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

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterInfoCache)]
    public class LobbyCharacterInfoCache : ISerializablePacket
    {
        [Raw4Bit(0)]
        public uint CacheIdentifier;
    }
}
