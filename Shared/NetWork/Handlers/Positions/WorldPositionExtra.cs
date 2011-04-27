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
        public long MapId = 177;

        [ListBit(3)]
        public List<float> Position = new List<float>() {1149965263,1147537926,1152778324} ; // X,Y,Z

        [ListBit(4)]
        public List<float> Field4 = new List<float>() { 2147483648, 3212777419, 2147483648, 3182182386 }; // Unk

        [Raw4Bit(5)]
        public uint Field5 = 1065772646;

        [ListBit(8)]
        public List<float> Field8 = new List<float>() { 1149965263, 1147537926, 1152778324 }; // Unk
    }
}
