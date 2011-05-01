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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterInfoDesc)]
    public class LobbyCharacterDesc : ISerializablePacket
    {
        [PacketBit(8)]
        public LobbyCharacterInfoCache Field8;
    }
}
