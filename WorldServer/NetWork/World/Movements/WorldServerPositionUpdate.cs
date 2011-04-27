using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldServerPositionUpdate)]
    public class WorldServerPositionUpdate : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;

        [ListBit(1)]
        public List<float> Position;

        [Unsigned7Bit(2)]
        public long UnkMovementInfo1 = 10569;

        [Unsigned7Bit(3)]
        public long UnkMovementInfo2 = 32767;

        [ListBit(4)]
        public List<float> Orientation;

        [BoolBit(12)]
        public bool UnkMovementInfo3;

        public override void OnRead(RiftClient From)
        {
            Log.Info("Movement Info", "Received client movement information");
        }
    }
}
