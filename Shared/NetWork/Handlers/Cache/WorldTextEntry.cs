using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Database;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldText_Info)]
    [Serializable]
    public class WorldText_Info : ISerializablePacket
    {
        [Raw4Bit(0)]
        public long Entry;

        [ArrayBit(1)]
        public string Text;
    }
}
