using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldMapInfo)]
    public class WorldMapInfo : ISerializablePacket
    {
        [Raw4Bit(0)]
        public uint MapId;

        [ArrayBit(1)]
        public string Text;
    }
}
