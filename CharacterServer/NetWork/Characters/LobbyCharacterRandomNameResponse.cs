using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreation_NameResponse)]
    public class LobbyCharacterCreation_NameResponse : ISerializablePacket
    {
        [ArrayBit(1)]
        public string Name;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
