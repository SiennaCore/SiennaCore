using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

namespace Shared
{
    [ISerializableAttribute((long)Opcodes.WorldZoneInfo)]
    [Serializable]
    public class WorldZoneInfo : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ZoneFileName;

        [PacketBit(1)]
        public WorldText_Info Description;

        [PacketBit(9)]
        public WorldText_Info DisplayName;

        
    }
}
