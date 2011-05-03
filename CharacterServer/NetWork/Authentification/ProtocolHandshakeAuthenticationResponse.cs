using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeAuthenticationResponse)]
    public class HandshakeAuthenticationResponse : ISerializablePacket
    {
        [Raw8BitAttribute(0)]
        public long SessionTicket;

        public override void OnRead(RiftClient From)
        {

        }
    }
}
