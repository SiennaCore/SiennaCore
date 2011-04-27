using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldChannelJoinned)]
    public class WorldPorticulumTeleport : ISerializablePacket
    {
        [ArrayBit(0)]
        public string MapName;

        [ArrayBit(1)]
        public string ZoneName;

        public override void OnRead(RiftClient From)
        {
            
        }
    }
}
