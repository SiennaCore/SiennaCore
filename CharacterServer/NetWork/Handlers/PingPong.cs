using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.ProtocolPingPong)]
    public class PingPong : ISerializablePacket
    {
        [Raw4Bit(0)]
        public UInt32 Timestamp;

        public override void OnRead(RiftClient From)
        {
            /*PingPong Pong = new PingPong();
            Pong.Timestamp = (UInt32)Timestamp;
            From.SendSerialized(Pong);*/
        }
    }
}
