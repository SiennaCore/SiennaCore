using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterListRequest)]
    public class LobbyCharacterListRequest : ISerializablePacket
    {

        public override void OnRead(RiftClient From)
        {
            Log.Success("CharacterListRequest", "Characters Requested For : " + From.GetIp);

            Character[] Chars = CharacterMgr.Instance.GetCharacters(From.Acct.Id, From.Realm.RealmId);

            LobbyCharacterListResponse Rp = new LobbyCharacterListResponse();
            foreach (Character Char in Chars)
            {
                Log.Debug("Data", Char.Data);

                XmlSerializer xs = new XmlSerializer(typeof(LobbyCharacterCustom));
                MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(Char.Data));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                LobbyCharacterCustom Custom = xs.Deserialize(memoryStream) as LobbyCharacterCustom;

                LobbyCharacterEntry Entry = new LobbyCharacterEntry();
                Entry.AccountId = Char.AccountId;
                Entry.Email = "email@hotmail.com";
                Entry.CharacterId = Char.Id;
                Entry.CharacterName = Char.Name;

                Entry.Field5 = new LobbyCharacterUnknown1();

                if (Char.Sex > 1)
                    Entry.Field5.AddField(2, EPacketFieldType.False , (bool)false);

                Entry.Field5.AddField(15, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Class);
                Entry.Field5.AddField(9, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Level);
                Entry.Field5.Custom = Custom;

                // 2 = Main Gauche
                // 3 = Bouclier

                /*{
                    // Main Gauche
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1170445023);
                    Test.AddField(5, EPacketFieldType.Raw4Bytes, (uint)2110727112);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)582720386);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 1933503643;

                    Entry.Field5.Field7.Add((long)2, Test);
                }*/

                /*{
                    // Shoulders
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1620638527);
                    Test.AddField(5, EPacketFieldType.Raw4Bytes, (uint)1791353197);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)582720386);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 2028933878;

                    Entry.Field5.Field7.Add((long)6, Test);
                }*/

                {
                    // Header
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1530909831);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)2);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 768949022;

                    Entry.Field5.Field7.Add((long)10, Test);
                }


                {
                    // Body
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1526924417);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)1495820655);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 1979672106;

                    Entry.Field5.Field7.Add((long)14, Test);
                }

                {
                    // Short
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1524239181);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)1495820655);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 632628948;

                    Entry.Field5.Field7.Add((long)16, Test);
                }

                {
                    // Hand
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)2002975902);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)582720386);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 699634871;

                    Entry.Field5.Field7.Add((long)18, Test);
                }

                {
                    // Feet
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)1533670656);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)1495820655);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 493821701;

                    Entry.Field5.Field7.Add((long)22, Test);
                }

                {
                    // Hairs
                    LobbyCharacterUnknown2 Test = new LobbyCharacterUnknown2();
                    Test.AddField(4, EPacketFieldType.Raw4Bytes, (uint)864322278);
                    Test.AddField(7, EPacketFieldType.Raw4Bytes, (uint)1785712051);
                    Test.Field8 = new LobbyCharacterUnknown3();
                    Test.Field8.CacheIdentifier = 807819347;

                    Entry.Field5.Field7.Add((long)46, Test);
                }

                Rp.Characters.Add(Entry);
            }

            From.SendSerialized(Rp);
        }

        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        } 

    }
}
