using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyWorldEntry)]
    public class LobbyWorldEntry : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long RealmID;

        [Unsigned7Bit(1)]
        public long Version;

        [BoolBit(3)]
        public bool PVP;

        [Unsigned7Bit(6)]
        public long CharactersCount;

        [Unsigned7Bit(11)]
        public long Population;

        [BoolBit(10)]
        public bool RP;

        [Unsigned7Bit(15)]
        public long Language;

        [BoolBit(16)]
        public bool Available = true;

        [BoolBit(17)]
        public bool Recommended;

        public override void OnRead(RiftClient From)
        {
           
        }
    }

    [ISerializableAttribute((long)Opcodes.LobbyWorldListResponse)]
    public class LobbyWorldListResponse : ISerializablePacket
    {
        [ListBit(1)]
        public List<ISerializablePacket> Realms = new List<ISerializablePacket>();

        public override void OnRead(RiftClient From)
        {

        }
    }
}
