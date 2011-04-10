using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Shared;

namespace CharacterServer
{
    public class ClientAuthCertificate
    {
        public string Username;
        public string Hash;
        public string Sessionkey;
    }

    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeAuthenticationRequest)]
    public class ProtocolHandshakeAuthenticationRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Signature;

        [ArrayBit(1)]
        public string Certificate;

        [ArrayBit(2)]
        public string Unk;

        public override void OnRead(RiftClient From)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
                ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

                if (Program.Config.UseCertificate)
                    From.Acct = Program.AcctMgr.GetAccountBySession(Cert.Sessionkey);
                else
                    From.Acct = Program.AcctMgr.GetAccountByPassword(Cert.Username, Cert.Hash);

            }
            catch (Exception e)
            {
                From.Disconnect();
            }

            if (From.Acct == null)
            {
                Log.Error("Authentication", "Failled !");
                From.Disconnect();
            }
            else
            {
                HandshakeAuthenticationResponse Rp = new HandshakeAuthenticationResponse();
                Rp.Success = new byte[] { 0x5D, 0x46, 0x7C, 0x07, 0x12, 0xC1, 0xA5, 0x30 };
                From.SendSerialized(Rp);
            }
        }
    }
}
