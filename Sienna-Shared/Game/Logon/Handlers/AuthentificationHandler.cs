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
        [LogonPacket((ushort)LogonOpcodes.Client_AuthCertificate)]
        public static void HandleAuthCertificate(LogonClient From, PacketStream Data)
        {
            Data.Skip(17);

            string Certificate = Data.ReadString((int)Data.Length());

            // Inform client that we are starting compression now
            PacketStream ps = new PacketStream();
            ps.ToLogonPacket((ushort)LogonOpcodes.Server_ZCompressStart);
            From.Send(LogonOpcodes.Server_ZCompressStart, ps);
            From.ServerCompressPackets = true;

            //XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
            //ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

            ReplyAuthCertificate(From, true/*From, Cert.IsValid(LogonConfig.get.UsingCustomCertificateServer)*/);
        }

        public static void ReplyAuthCertificate(LogonClient To, bool Result)
        {
            if (!Result)
                return;

            PacketStream ps = new PacketStream();
            ps.WriteUInt32(0xCF96D10D); // Unk1
            ps.WriteUInt32(0xF2336D3C); // Unk2
            ps.WriteUInt16(0x0749); // Unk3

            To.Send(LogonOpcodes.Server_AuthCertificate, ps);

            ps = new PacketStream();
            ps.WriteUInt32(0x0067FDEF);
            ps.WriteByte(0x07);
            To.Send(LogonOpcodes.Server_Unk1, ps);
        }
    }
}
