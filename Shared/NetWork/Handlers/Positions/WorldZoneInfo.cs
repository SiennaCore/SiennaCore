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
        public WorldTextEntry Description;

        [PacketBit(9)]
        public WorldTextEntry DisplayName;

        
    }
}
