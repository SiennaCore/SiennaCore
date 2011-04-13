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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterUnknown1)]
    public class LobbyCharacterUnknown1 : ISerializablePacket
    {
        //[Raw4Bit(0)]
        //public uint Field0 = 1800830726;

        //[Unsigned7Bit(1)]
        //public long Field1 = 3;

        //[BoolBit(2)]
        //public bool Male = false;

        //[Raw4Bit(4)]
        //public uint Field4 = 290412351;

        //[Raw4Bit(5)]
        //public uint Field5 = 777065106;

        //[Raw4Bit(6)]
        //public uint Field6 = 1033425102;

        //[DicBit(7)]
        //public Dictionary<long, ISerializablePacket> Field7 = new Dictionary<long, ISerializablePacket>(); // Equipement

        //[Unsigned7Bit(8)]
        //public long Field8 = 8;

        //[Unsigned7Bit(9)]
        //public long Field9;
        // Level

        //[Unsigned7Bit(15)]
        //public long Field15 = 3; 
        // Classe 3 = Mage

        //[Raw4Bit(16)]
        //public uint Field16 = 15;

        //[Raw4Bit(17)]
        //public uint Field17 = 1712948866;

        //[ArrayBit(18)]
        //public string MapName = "guardian_map";

        //[Unsigned7Bit(19)]
        //public long Field19 = 19;

        //[PacketBit(21)]
        //public LobbyCharacterCustom Custom;

        //[ListBit(24)]
        //public List<uint> Field24 = new List<uint>( new uint[] {1149965263, 1147537107, 1152778324} );

        //[ListBit(25)]
        //public List<ISerializablePacket> Field25 = new List<ISerializablePacket>();

        //[Unsigned7Bit(26)]
        //public long Field26 = 60006;

    }
}
