using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeServerKey)]
    public class HandshakeServerKey : ISerializablePacket
    {
        [Encoded7BitAttribute(0)]
        public long Algorithm;

        [Encoded7BitAttribute(1)]
        public long ServerKey;

        [ArrayBit(2)]
        public byte[] Unknown;

        public override void OnRead(RiftClient From)
        {
        }
    }
}
