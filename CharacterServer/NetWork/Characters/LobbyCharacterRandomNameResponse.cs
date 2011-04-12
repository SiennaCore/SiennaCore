using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterRandomNameResponse)]
    public class LobbyCharacterRandomNameResponse : ISerializablePacket
    {
        [ArrayBit(1)]
        public string Name;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
