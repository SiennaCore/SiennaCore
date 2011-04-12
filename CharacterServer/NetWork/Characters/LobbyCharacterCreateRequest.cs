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
    [Serializable]
    [ISerializableAttribute((long)Opcodes.LobbyCharacterUnknown4)]
    public class LobbyCharacterUnknown4 : ISerializablePacket
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
    public class LobbyCharacterCreateRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Name;

        [Unsigned7Bit(1)]
        public long Field1;

        [Unsigned7Bit(2)]
        public long Field2;

        [Raw4Bit(4)]
        public uint Field4;

        [Raw4Bit(5)]
        public uint Field5;

        [BoolBit(6)]
        public byte Field6;

        [Unsigned7Bit(8)]
        public long Field8;

        [Unsigned7Bit(9)]
        public long Field9;

        [Raw4Bit(10)]
        public uint Field10;

        [Unsigned7Bit(13)]
        public long Field13;

        [PacketBit(28)]
        public LobbyCharacterUnknown4 Custom;

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
            Log.Success("Creation", "CharacterName  = " + Name);

            string Data = "";

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(LobbyCharacterCreateRequest));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, this);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            Data = UTF8ByteArrayToString(memoryStream.ToArray());


            Character Char = new Character();
            Char.Name = Name;
            Char.AccountId = From.Acct.Id;
            Char.RealmId = From.Realm.RealmId;
            Char.Data = Data;
            Program.CharMgr.AddObject(Char);

        }

        private String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

    }
}
