using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.ProtocolPingPong)]
    public class PingPong : ISerializablePacket
    {
        [Raw4Bit(0)]
        public long Timestamp;

        public override void OnRead(RiftClient From)
        {
            Log.Success("Ping", "Time=" + Timestamp + "," + Environment.TickCount);
            PingPong Pong = new PingPong();
            Pong.Timestamp = (UInt32)Timestamp;
            From.SendSerialized(Pong);
        }
    }
}
