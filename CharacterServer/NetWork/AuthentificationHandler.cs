using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.NetWork;

namespace CharacterServer
{
    public class ClientAuthCertificate
    {
        public string Username;
        public string Hash;
        public string Sessionkey;
    }

    public class Authentification : IPacketHandler
    {
        [PacketHandler(PacketHandlerType.TCP,(int)Opcodes.AUTHENTIFICATION,(int)RiftState.CONNECTING,"AUTHENTIFICATION")]
        static public void HandleCertificate(BaseClient client,PacketIn Packet)
        {
            RiftClient Client = client as RiftClient;

            Packet.Skip(16);

            string Certificate = Packet.GetString((int)Packet.Remain());
            string EndTag = "</ClientAuthCertificate>";
            Certificate = Certificate.Substring(0, Certificate.IndexOf(EndTag) + EndTag.Length);


            // Check certificate validity
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
                ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

                if (Program.Config.UseCertificate)
                    Client.Acct = Program.AcctMgr.GetAccountBySession(Cert.Sessionkey);
                else
                    Client.Acct = Program.AcctMgr.GetAccountByPassword(Cert.Username, Cert.Hash);

            }
            catch (Exception e)
            {
                Log.Error("Authentification", e.ToString());
                client.Disconnect();
                return;
            }

            if (Client.Acct == null)
                Client.Disconnect();
            else
            {
                Client.State = (int)RiftState.AUTHENTIFIED;

                EnableCompresion(Client);
                ReplyAuthentification(Client);

                PacketOut Unk = new PacketOut((ushort)0x0402);
                Unk.WriteUInt32R(0x0067FDEF);
                Unk.WriteByte(7);
                Client.SendTCP(Unk);

            }
        }
        static public void EnableCompresion(RiftClient Client)
        {
            PacketOut Out = new PacketOut((ushort)Opcodes.START_COMPRESSION);
            Client.SendTCP(Out);

            Client.InitZlib();
        }
        static public void ReplyAuthentification(RiftClient Client)
        {
            PacketOut Out = new PacketOut((ushort)Opcodes.AUTHENTIFICATION_RESPONSE);
            Out.WriteUInt32R(0xCF96D10D);
            Out.WriteUInt32R(0xF2336D3C);
            Out.WriteUInt32R(0x0749);
            Client.SendTCP(Out);
        }

        [PacketHandler(PacketHandlerType.TCP, (int)Opcodes.PING, (int)RiftState.CONNECTING, "PING")]
        static public void HandlePing(BaseClient client, PacketIn Packet)
        {

        }
    }
}
