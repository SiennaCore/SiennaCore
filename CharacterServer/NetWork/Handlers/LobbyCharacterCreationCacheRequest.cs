using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreationCacheRequest)]
    public class LobbyCharacterCreationCacheRequest : ISerializablePacket
    {
        public override void OnRead(RiftClient From)
        {
            Log.Success("CreationCache", "Request");
        }
    }
}
