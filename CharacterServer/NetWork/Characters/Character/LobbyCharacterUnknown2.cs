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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterUnknown2)]
    public class LobbyCharacterUnknown2 : ISerializablePacket
    {
        [PacketBit(8)]
        public LobbyCharacterUnknown3 Field8;
    }
}
