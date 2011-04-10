using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyWorldListRequest)]
    public class LobbyWorldListRequest : ISerializablePacket
    {
        public override void OnRead(RiftClient From)
        {
            Log.Success("WorldList", "Request : In Progress");

            LobbyWorldListResponse Rp = new LobbyWorldListResponse();
            From.SendSerialized(Rp);
        }
    }
}
