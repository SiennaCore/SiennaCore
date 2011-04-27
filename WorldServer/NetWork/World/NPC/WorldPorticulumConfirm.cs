using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldPorticulumConfirm)]
    public class WorldPorticulumConfirm : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long Cost;

        [ArrayBit(1)]
        public string ZoneData;

        [Raw8Bit(2)]
        public long NPCId;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
