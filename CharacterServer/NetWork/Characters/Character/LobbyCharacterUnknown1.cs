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
        //public uint Field0 = 0;

        [Unsigned7Bit(1)]
        public long Field1 = 3;
        // Race
        // 1 = Mathosien
        // 2 = Haut Elf
        // 3 = Nain

        //[BoolBit(2)]
        //public bool Male = true;

        [Raw4Bit(4)]
        public uint Field4 = 290412351;
        // MapID

        [Raw4Bit(5)]
        public uint Field5 = 777065106;
        // Change Corp

        [Raw4Bit(6)]
        public uint Field6 = 1798083889;
        // Change Corp
        // 33 = Mathosien Homme
        // 1231885383 = HautElf Homme
        // 1703344031 = Nain Homme

        // 3 = Mathosien Femme
        // 1798083889 = HautElf Femme
        // 1033425102 = Nain Femme

        [DicBit(7)]
        public Dictionary<long, ISerializablePacket> Field7 = new Dictionary<long, ISerializablePacket>(); // Models

        [Unsigned7Bit(8)]
        public long Field8 = 10;
        // 10 = Gardien Homme && Femme
        // 4 = Nain Femme


        //[Unsigned7Bit(9)]
        //public long Field9;
        // Level

        //[Unsigned7Bit(15)]
        //public long Field15 = 3; 
        // Classe
        // 1 = Guerrier
        // 2 = Clerc
        // 3 = Mage
        // 4 = Voleur

        [Unsigned7Bit(16)]
        public long Field16 = 15;
        // 15 = Gardien Homme
        // 15 = Gardien Femme

        [Raw4Bit(17)]
        public uint Field17 = 1712948866;
        // 1712948866 = Gardien Homme && Femme

        [ArrayBit(18)]
        public string MapName = "guardian_map";

        [Unsigned7Bit(19)]
        public long Field19 = 177;
        // 177 = Gardien Homme && Femme

        [PacketBit(21)]
        public LobbyCharacterCustom Custom;

        [ListBit(24)]
        public List<uint> Field24 = new List<uint>( new uint[] {1149965263, 1147537107, 1152778324} );
        // Mathosien = 1149965263, 1147537107 , 1152778324
        // Haut Elf = 1149965263, 1147537107 , 1152778324
        // Nain = 1149980776, 1147537294, 1152655995
        // Nain Fille = 1149980776, 1147537294, 1152655995

        [ListBit(25)]
        public List<ISerializablePacket> Field25 = new List<ISerializablePacket>();

        [Unsigned7Bit(26)]
        public long Field26 = 80000;
        // Mathosien = (Homme 80008) && (Femme 60006)
        // Haut Elf =   (Homme 80000) && (Femme 80008)
        // Nain =       80000

    }
}
