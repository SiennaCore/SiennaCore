using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeServerKey)]
    public class HandshakeServerKey : ISerializablePacket
    {
        [Unsigned7BitAttribute(0)]
        public long Nid;

        [ArrayBit(1)]
        public byte[] ServerKey;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
