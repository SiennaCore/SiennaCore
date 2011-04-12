using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterListRequest)]
    public class LobbyCharacterListRequest : ISerializablePacket
    {

        public override void OnRead(RiftClient From)
        {
            Log.Success("CharacterListRequest", "Characters Requested For : " + From.GetIp);

            LobbyCharacterListResponse Rp = new LobbyCharacterListResponse();
            From.SendSerialized(Rp);
        }
    }
}
