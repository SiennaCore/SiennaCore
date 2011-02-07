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
            Data.Skip(16);

            // Get certificate
            string Certificate = Data.ReadString((int)Data.Length());
            string EndTag = "</ClientAuthCertificate>";
            Certificate = Certificate.Substring(0, Certificate.IndexOf(EndTag) + EndTag.Length);

            // Inform client that we are starting compression now
            PacketStream ps = new PacketStream();
            ps.ToLogonPacket((ushort)LogonOpcodes.Server_ZCompressStart);
            From.Send(LogonOpcodes.Server_ZCompressStart, ps);
            From.ServerCompressPackets = true;

            // Check certificate validity
            XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
            ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

            ReplyAuthCertificate(From, Cert.IsValid(LogonConfig.get.UsingCustomCertificateServer));
        }

        [LogonPacket((ushort)LogonOpcodes.Client_RequestRealmlist)]
        public static void HandleCharSelectionRequest(LogonClient From, PacketStream Data)
        {
            PacketStream ps = new PacketStream();

            // Unk
            ps.WriteUInt16Reversed(0x5F97);

            int RealmCount = Realm.Realmlist.Count;

            // Check if realm count is multiple of 2
            bool isMultipleOfTwo = RealmCount % 2 == 0 ? true : false;

            // If it's not a multiple of 2 add 1
            if (!isMultipleOfTwo && RealmCount > 0)
                RealmCount++;

            // Write realmcount / 2
            ps.WriteByte((byte)(RealmCount / 2));
            
            // Write realms data
            foreach (Realm r in Realm.Realmlist)
                ps.Write(r.ToLoginData(true));

            // If realm count is not multiple of two add again the first realm
            if (!isMultipleOfTwo && Realm.Realmlist.Count > 0)
                ps.Write(Realm.Realmlist[0].ToLoginData(false));

            // End
            ps.WriteByte(0x07);

            From.Send(LogonOpcodes.Server_SendRealmlist, ps);
        }

        [LogonPacket((ushort)LogonOpcodes.Client_SelectRealm)]
        public static void HandleSelectRealmRequest(LogonClient From, PacketStream Data)
        {
            PacketStream ps = new PacketStream();
            ps.WriteByte(0x07);

            From.Send(LogonOpcodes.Client_SelectRealm, ps);
            From.Send(TempHacks.AllowCharCreation);
        }

        [LogonPacket((ushort)LogonOpcodes.Client_RequestCharacterlist)]
        public static void HandleCharlistRequest(LogonClient From, PacketStream Data)
        {
            From.Send(TempHacks.CharacterInfos);
        }

        [LogonPacket((ushort)LogonOpcodes.Client_RequestCharCreationInfos)]
        public static void HandleCharCreationRequest(LogonClient From, PacketStream Data)
        {
            From.Send(TempHacks.CharCreationInfos);
        }

        public static void ReplyAuthCertificate(LogonClient To, Account Result)
        {
            if (Result == null)
            {
                To.Disconnect();
                return;
            }

            To.Acct = Result;

            PacketStream ps = new PacketStream();
            ps.WriteUInt32(0xCF96D10D); // Unk1
            ps.WriteUInt32(0xF2336D3C); // Unk2
            ps.WriteUInt16(0x0749); // Unk3

            To.Send(LogonOpcodes.Server_AuthCertificate, ps);
            To.Ping();
        }
    }
}
