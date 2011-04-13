using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeVersion)]
    public class HandshakeVersion : ISerializablePacket
    {
        [Encoded7Bit(0)]
        public long Version;

        public override void OnRead(RiftClient From)
        {
            Log.Success("HanshakeVersion", "Version = " + Version);

            From.ClientVersion = Version;

            HandshakeCompression Cp = new HandshakeCompression();
            Cp.Enabled = true;
            From.SendSerialized(Cp);

            From.EnableSendCompress();
        }
    }
}
