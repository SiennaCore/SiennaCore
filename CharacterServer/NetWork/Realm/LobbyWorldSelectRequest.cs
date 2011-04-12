using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyWorldSelectRequest)]
    public class LobbyWorldSelectRequest : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long RealmId;

        public override void OnRead(RiftClient From)
        {
            From.Realm = Program.CharMgr.GetRealm((int)RealmId);

            if (From.Realm == null)
            {
                Log.Error("LobbyWorldSelectRequest", "Invalid RealmId=" + RealmId);
                From.Disconnect();
            }
            else
            {
                LobbyWorldSelectResponse Rp = new LobbyWorldSelectResponse();
                From.SendSerialized(Rp);
            }
        }
    }
}
