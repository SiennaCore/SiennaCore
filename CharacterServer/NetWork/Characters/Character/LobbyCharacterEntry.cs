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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterEntry)]
    public class LobbyCharacterEntry : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long AccountId;

        [ArrayBit(1)]
        public string Email;

        [Raw8Bit(2)]
        public long CharacterId; // Character ID

        [ArrayBit(3)]
        public string CharacterName;

        [Unsigned7Bit(4)]
        public long Field4 = 3;

        [PacketBit(5)]
        public LobbyCharacterInfoBase Field5;

        [BoolBit(6)]
        public bool Field6 = true;

        [Raw8Bit(7)]
        public long Field7 = 129472084607000000;
    }
}
