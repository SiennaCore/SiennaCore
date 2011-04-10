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

            {
                LobbyWorldEntry Entry = new LobbyWorldEntry();
                Entry.RealmID = 2563;
                Entry.Version = 29037;
                Entry.PVP = false;
                Entry.RP = false;
                Entry.CharactersCount = 1;
                Entry.Population = 2;
                Entry.Recommended = false;
                Entry.Online = true;
                Rp.Realms.Add(Entry);
            }

            {
                LobbyWorldEntry Entry = new LobbyWorldEntry();
                Entry.RealmID = 2564;
                Entry.Version = 29034;
                Entry.PVP = false;
                Entry.RP = false;
                Entry.CharactersCount = 1;
                Entry.Population = 0;
                Entry.Recommended = true;
                Entry.Online = true;
                Rp.Realms.Add(Entry);
            }

            From.SendSerialized(Rp);
        }
    }
}
