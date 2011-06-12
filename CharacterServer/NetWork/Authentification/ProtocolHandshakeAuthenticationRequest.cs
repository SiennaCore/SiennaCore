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
                    From.Acct = AccountMgr.Instance.GetAccountBySession(Cert.Sessionkey);
                else
                    From.Acct = AccountMgr.Instance.GetAccountByPassword(Cert.Username, Cert.Hash);

            }
            catch
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
                Rp.SessionTicket = (long)BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);

                From.Acct.SessionTicket = Rp.SessionTicket;
                From.Acct.Dirty = true;
                AccountMgr.AccountDB.SaveObject(From.Acct);

                From.SendSerialized(Rp);
            }
        }
    }
}
