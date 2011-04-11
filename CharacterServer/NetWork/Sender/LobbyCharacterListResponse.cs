using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterListResponse)]
    public class LobbyCharacterListResponse : ISerializablePacket
    {
        [ListBit(1)]
        public List<ISerializablePacket> Characters = new List<ISerializablePacket>();

        [Unsigned7Bit(4)]
        public long Field4 = 100;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
