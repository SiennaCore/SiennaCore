using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreateResponse)]
    public class LobbyCharacterCreateResponse : ISerializablePacket
    {

    }

    [Serializable]
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCustom)]
    public class LobbyCharacterCustom : ISerializablePacket
    {
        [ListBit(1)]
        public List<uint> Field1;

        [ListBit(2)]
        public List<uint> Field2;

        [ListBit(3)]
        public List<uint> Field3;

        [ListBit(4)]
        public List<uint> Field4;

        [ListBit(5)]
        public List<uint> Field5;

        [ListBit(6)]
        public List<uint> Field6;

        [ListBit(7)]
        public List<uint> Field7;

        [Unsigned7Bit(8)]
        public long Field8;

        [Unsigned7Bit(9)]
        public long Field9;

        [Unsigned7Bit(10)]
        public long Field10;

        [Unsigned7Bit(11)]
        public long Field11;

        [Unsigned7Bit(12)]
        public long Field12;

        [ListBit(14)]
        public List<uint> Field14;

        [ArrayBit(15)]
        public string Field15;

        [ArrayBit(16)]
        public string Field16;

        [Raw4Bit(17)]
        public uint Field17;

        [Raw4Bit(18)]
        public uint Field18;

        [Raw4Bit(19)]
        public uint Field19;

        [Raw4Bit(20)]
        public uint Field20;

        [Raw4Bit(21)]
        public uint Field21;

        [Raw4Bit(22)]
        public uint Field22;

        [Raw4Bit(23)]
        public uint Field23;

        [ListBit(25)]
        public List<uint> Field25;
    }

    [Serializable]
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreateRequest)]
    [XmlRootAttribute(ElementName = "Custom", IsNullable = false)]
    public class LobbyCharacterCreateRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Name;

        [Unsigned7Bit(1)]
        public long Field1=1;
        // Race + Sex

        [Unsigned7Bit(2)]
        public long Field2=0;

        [Raw4Bit(4)]
        public uint Field4;

        [Raw4Bit(5)]
        public uint Field5;

        [Raw4Bit(6)]
        public uint Field6;

        [Unsigned7Bit(8)]
        public long Field8;

        [Unsigned7Bit(9)]
        public long Field9;

        [Raw4Bit(10)]
        public uint Field10;

        [Unsigned7Bit(13)]
        public long Field13=1; // Classe

        [PacketBit(28)]
        public LobbyCharacterCustom Custom;

        [Unsigned7Bit(29)]
        public long Field29;

        [Unsigned7Bit(30)]
        public long Field30;

        [Unsigned7Bit(31)]
        public long Field31;

        [Unsigned7Bit(32)]
        public long Field32;

        [Unsigned7Bit(33)]
        public long Field33;

        [Unsigned7Bit(34)]
        public long Field34;

        [Unsigned7Bit(35)]
        public long Field35;

        public override void OnRead(RiftClient From)
        {
            if (CharacterMgr.Instance.GetCharactersCount(From.Acct.Id, From.Realm.RealmId) >= 6)
            {
                Log.Error("CharacterCreate", "Hack From : " + From.GetIp);
                From.Disconnect();
                return;
            }

            LobbyCharacterCreateResponse Rp = new LobbyCharacterCreateResponse();
            Character Exist = CharacterMgr.CharacterDB.SelectObject<Character>("Name='" + CharacterMgr.CharacterDB.Escape(Name) + "' AND RealmID=" + From.Realm.RealmId);
            if (Exist != null)
            {
                Rp.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)6);
            }
            else
            {
                Rp.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)0);
                string Data = "";

                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(LobbyCharacterCustom));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, Custom);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                Data = UTF8ByteArrayToString(memoryStream.ToArray());

                Character Char = new Character();
                Char.Name = Name;
                Char.AccountId = From.Acct.Id;
                Char.RealmId = From.Realm.RealmId;
                Char.Race = Field1;
                Char.Sex = Field2;
                Char.Class = Field13;
                Char.Level = 1;
                Char.Data = Data;

                CharacterMgr.Instance.AddObject(Char);
            }
            From.SendSerialized(Rp);
        }

        private String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return constructedString.Remove(0,1);
        }

    }
}
