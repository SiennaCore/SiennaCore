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
            Realm Rm = Program.CharMgr.GetRealm((int)RealmId);

            if (Rm != null)
                From.Realm = Rm;

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
