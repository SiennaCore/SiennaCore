using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldZoneInfo)]
    public class WorldZoneInfo : ISerializablePacket
    {
        [ArrayBit(0)]
        public string MapFileName;
    }
}
