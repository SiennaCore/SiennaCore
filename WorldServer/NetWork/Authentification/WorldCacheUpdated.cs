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
    [ISerializableAttribute((long)Opcodes.WorldCacheUpdated)]
    public class WorldCacheUpdated : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;
    }
}
