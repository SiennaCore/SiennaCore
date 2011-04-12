using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterRandomNameRequest)]
    public class LobbyCharacterRandomNameRequest : ISerializablePacket
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
            LobbyCharacterRandomNameResponse Rp = new LobbyCharacterRandomNameResponse();
            Rp.Name = Program.CharMgr.GenerateName();
            From.SendSerialized(Rp);
        }
    }
}
