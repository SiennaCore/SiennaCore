using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldStartingPosition)]
    public class WorldStartingPosition : ISerializablePacket
    {
        [ArrayBit(0)]
        public string MapName;

        [ListBit(1)]
        public List<uint> Position = new List<uint>();
    }
}
