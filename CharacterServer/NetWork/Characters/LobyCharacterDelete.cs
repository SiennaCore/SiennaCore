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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterDelete)]
    public class LobbyCharacterDelete : ISerializablePacket
    {
        [ArrayBit(0)]
        public string UnkText;

        [Raw8Bit(1)]
        public long GUID;

        public override void OnRead(RiftClient From)
        {
            // Client requested delete character with guid
        }
    }
}
