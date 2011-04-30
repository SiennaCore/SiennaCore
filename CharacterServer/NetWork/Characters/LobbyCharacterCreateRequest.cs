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

    }

    [Serializable]
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreateRequest)]
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

        [Raw4Bit(10)]
        public uint Field10;

        [Unsigned7Bit(13)]
        public long Field13=1; // Classe

        [PacketBit(28)]
        public LobbyCharacterCustom Custom;

        public byte[] CustomData;

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

                PacketOutStream Stream = new PacketOutStream();
                PacketProcessor.WritePacket(ref Stream, Custom, false, true, true);
                CustomData = Stream.ToArray();

                string Data = UTF8ByteArrayToString(CustomData);
                
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

        static public String UTF8ByteArrayToString(byte[] characters)
        {

            string Result = "";
            foreach (byte b in characters)
                Result += b.ToString() + " ";

            Result = Result.Remove(Result.Length - 1, 1);
            return Result;
        }

    }
}
