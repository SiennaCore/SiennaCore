using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldServerMOTD)]
    public class WorldServerMOTD : ISerializablePacket
    {
        [ArrayBit(1)]
        public string Text;
    }
}
