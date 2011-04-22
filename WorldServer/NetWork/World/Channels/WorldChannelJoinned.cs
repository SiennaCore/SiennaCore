using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldChannelJoinned)]
    public class WorldChannelJoinned : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ChannelName;

        [ArrayBit(1)]
        public string CharacterName;

        [Unsigned7Bit(2)]
        public long Field2 = 5;
    }
}
