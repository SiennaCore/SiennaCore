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

            Realm[] Realms = Program.CharMgr.GetRealms();
            foreach (Realm Rm in Realms)
            {
                LobbyWorldEntry Entry = new LobbyWorldEntry();
                Entry.RealmID = Rm.RiftId;
                Entry.PVP = Rm.PVP == 1;
                Entry.Recommended = Rm.Recommended == 1;
                Entry.Population = 0;
                Entry.RP = Rm.RP == 1;
                Entry.Version = Rm.Version;
                Entry.Online = Rm.Online == 1;
                Entry.CharactersCount = 0;
                Entry.Language = Rm.Language;
                Rp.Realms.Add(Entry);
            }

            From.SendSerialized(Rp);
        }
    }
}
