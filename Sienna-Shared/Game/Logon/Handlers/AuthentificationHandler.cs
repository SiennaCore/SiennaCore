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

        [LogonPacket((ushort)LogonOpcodes.Client_RequestRealmlist)]
        public static void HandleCharSelectionRequest(LogonClient From, PacketStream Data)
        {
            /*PacketStream ps = new PacketStream();
            ps.WriteByte(0x03);
            ps.WriteByte(0x5F);
            ps.WriteByte(0x97);
            ps.WriteByte(0x0B);
            ps.WriteByte(0xE5);
            ps.WriteByte(0x03);
            ps.WriteByte(0x02);
            ps.WriteByte(0xEB);
            ps.WriteByte(0x11);
            ps.WriteByte(0x0A);
            ps.WriteByte(0xB6);
            ps.WriteByte(0xC3);
            ps.WriteByte(0x01);
            ps.WriteByte(0x07);

            From.Send(LogonOpcodes.Server_SendRealmlist , ps);*/

            From.Send(TempHacks.RealmlistInfos);
        }

        [LogonPacket((ushort)LogonOpcodes.Client_SelectRealm)]
        public static void HandleSelectRealmRequest(LogonClient From, PacketStream Data)
        {
            PacketStream ps = new PacketStream();
            ps.WriteByte(0x07);
            From.Send(LogonOpcodes.Server_AcceptRealmSelection, ps);
        }

        [LogonPacket((ushort)LogonOpcodes.Client_RequestCharacterlist)]
        public static void HandleCharlistRequest(LogonClient From, PacketStream Data)
        {

        }

        [LogonPacket((ushort)LogonOpcodes.Client_CreateCharacter)]
        public static void HandleCharCreationRequest(LogonClient From, PacketStream Data)
        {
            From.Send(TempHacks.CharCreationInfos);
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
            To.Ping();
        }
    }
}
