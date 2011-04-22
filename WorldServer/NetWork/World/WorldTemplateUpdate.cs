using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldTemplateUpdate)]
    public class WorldTemplateUpdate : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;

        [ArrayBit(1)]
        public byte[] Field1;
    }
}
