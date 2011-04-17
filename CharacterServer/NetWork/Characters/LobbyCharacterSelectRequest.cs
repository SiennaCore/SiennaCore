using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterSelectResponse)]
    public class LobbyCharacterSelectResponse : ISerializablePacket
    {
        [ListBit(1)]
        public List<string> Ips = new List<string>();
    }

    [ISerializableAttribute((long)Opcodes.LobbyCharacterSelectRequest)]
    public class LobbyCharacterSelectRequest : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long Field0;

        public override void OnRead(RiftClient From)
        {
            Log.Success("SelectRequest","Enter on World : " + From.GetIp + ",Field="+Field0);

            LobbyCharacterSelectResponse Rp = new LobbyCharacterSelectResponse();
            if (From.Realm != null)
            {
                Log.Success("Entering", "On : " + From.Realm.Address);
                Rp.Ips.Add(From.Realm.Address);
            }

            From.SendSerialized(Rp);
        }
    }
}
