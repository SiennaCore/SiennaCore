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

            Character[] Chars = Program.CharMgr.GetCharacters(From.Acct.Id, From.Realm.RealmId);

            LobbyCharacterListResponse Rp = new LobbyCharacterListResponse();
            foreach (Character Char in Chars)
            {
                Log.Debug("Data", Char.Data);

                XmlSerializer xs = new XmlSerializer(typeof(LobbyCharacterCreateRequest));
                MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(Char.Data));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                LobbyCharacterCreateRequest Request = xs.Deserialize(memoryStream) as LobbyCharacterCreateRequest;

                LobbyCharacterEntry Entry = new LobbyCharacterEntry();
                Entry.AccountId = Char.AccountId;
                Entry.Email = "email@hotmail.com";
                Entry.CharacterId = Char.Id;
                Entry.CharacterName = Char.Name;
                Entry.Field5 = new LobbyCharacterUnknown1();
                Entry.Field5.AddField(15, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Classe);
                Entry.Field5.AddField(9, EPacketFieldType.Unsigned7BitEncoded, (long)Char.Level);
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
