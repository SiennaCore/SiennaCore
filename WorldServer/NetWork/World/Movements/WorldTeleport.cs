using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldChannelJoinned)]
    public class WorldTeleport : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long MapId;

        [ListBit(1)]
        public List<ISerializablePacket> DestinationData;

        public override void OnRead(RiftClient From)
        {
            
        }
    }
}
