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
        public List<uint> Position = new List<uint>() { 1149965263,1147537926,1152778324}; // X,Y,Z
    }
}
