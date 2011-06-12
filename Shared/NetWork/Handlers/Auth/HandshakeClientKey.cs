using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeClientKey)]
    public class HandshakeClientKey : ISerializablePacket
    {
        [ArrayBitAttribute(0)]
        public byte[] ClientKey;

        [ArrayBit(1)]
        public byte[] Unknown;

        [ArrayBit(2)]
        public byte[] Unknown1;

        [Encoded7BitAttribute(3)]
        public long Version;

        public override void OnRead(RiftClient From)
        {
            Log.Dump("ClientKey", ClientKey, 0, ClientKey.Length);

            From.InitCrypto(ClientKey);

            HandshakeCompression Cp = new HandshakeCompression();
            Cp.Enabled = true;
            From.SendSerialized(Cp);
            From.EnableSendCompress();


            HandshakeServerKey ServerKey = new HandshakeServerKey();
            ServerKey.Algorithm = 2980549511;
            ServerKey.ServerKey = 3061945505;
            ServerKey.Unknown = From.LocalPublicKey;
            From.SendSerialized(ServerKey);
        }
    }
}
