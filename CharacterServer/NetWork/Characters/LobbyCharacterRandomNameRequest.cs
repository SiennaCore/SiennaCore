using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreation_NameRequest)]
    public class LobbyCharacterCreation_NameRequest : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long Field0;

        [Unsigned7Bit(1)]
        public long Field1;

        [Unsigned7Bit(2)]
        public long Field2;

        [Unsigned7Bit(3)]
        public long Field3;

        [Unsigned7Bit(4)]
        public long Field4;

        public override void OnRead(RiftClient From)
        {
            LobbyCharacterCreation_NameResponse Rp = new LobbyCharacterCreation_NameResponse();
            Rp.Name = CharacterMgr.Instance.GenerateName();
            From.SendSerialized(Rp);
        }
    }
}
