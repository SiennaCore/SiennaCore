using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldStartingPosition)]
    public class WorldStartingPosition : ISerializablePacket
    {
        [ArrayBit(0)]
        public string MapName;

        [ListBit(1)]
        public List<float> Position = new List<float>() { 1113.03967f, 920.1114f, 1444.58533f }; // X,Y,Z
    }
}
