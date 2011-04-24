using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldPositionExtra)]
    public class WorldPositionExtra : ISerializablePacket
    {
        [ArrayBit(1)]
        public string MapName;

        [Unsigned7Bit(2)]
        public long Field2 = 177;

        [ListBit(3)]
        public List<uint> Position = new List<uint>() {1149965263,1147537926,1152778324} ; // X,Y,Z

        [ListBit(4)]
        public List<uint> Field4 = new List<uint>() { 2147483648, 3212777419, 2147483648, 3182182386 }; // Unk

        [Raw4Bit(5)]
        public uint Field5 = 1065772646;

        [ListBit(4)]
        public List<uint> Field8 = new List<uint>() { 1149965263, 1147537926, 1152778324 }; // Unk
    }
}
