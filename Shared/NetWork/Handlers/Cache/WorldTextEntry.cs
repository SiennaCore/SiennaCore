using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Database;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldTextEntry)]
    [Serializable]
    public class WorldTextEntry : ISerializablePacket
    {
        [Raw4Bit(0)]
        public long Entry;

        [ArrayBit(1)]
        public string Text;
    }
}
