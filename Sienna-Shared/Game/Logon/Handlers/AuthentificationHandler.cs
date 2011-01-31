using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;

using Sienna.Network;

namespace Sienna.Game
{
    public static class AuthentificationHandler
    {
        [LogonPacket((ushort)LogonOpcodes.HandleAuthCertificate)]
        public static void HandleAuthCertificate(LogonClient From, PacketStream Data)
        {
            Data.Skip(17);

            string Certificate = Data.ReadString((int)Data.Length());

            XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
            ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

            ReplyAuthCertificate(From, Cert.IsValid(LogonConfig.get.UsingCustomCertificateServer));
        }

        public static void ReplyAuthCertificate(LogonClient To, bool Result)
        {
            PacketStream ps = new PacketStream();
            ps.Write(new byte[] { 0x0D, 0x3E, 0xD5, 0xE8, 0x44, 0x0E, 0xCA, 0x67, 0x5D, 0x07 });

            To.Send((ushort)LogonOpcodes.ReplyAuthCertificate, ps);
        }
    }
}
