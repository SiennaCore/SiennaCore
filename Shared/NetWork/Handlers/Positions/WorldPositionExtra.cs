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
        public List<float> Position = new List<float>() { 1113.03967f, 920.1114f, 1444.58533f }; // X,Y,Z

        // Unk
        [ListBit(4)]
        public List<uint> Field4 = new List<uint>() { 2147483648, 3212777419, 2147483648, 3182182386 }; // Unk

        [Raw4Bit(5)]
        public uint Field5 = 1065772646;

        // Zone Offset ?
        [ListBit(8)]
        public List<float> Position2 = new List<float>() { 1113.03967f, 920.1114f, 1444.58533f };
    }
}
