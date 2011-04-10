using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeCompression)]
    public class HandshakeCompression : ISerializablePacket
    {
        [BoolBitAttribute(0)]
        public bool Enabled;

        public override void OnRead(RiftClient From)
        {
            From.EnableReceiveCompress();
        }
    }
}
