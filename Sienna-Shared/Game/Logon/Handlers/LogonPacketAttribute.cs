using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna.Game
{
    public class LogonPacketAttribute : Attribute
    {
        public UInt16 Opcode;

        public LogonPacketAttribute(UInt16 _Opcode)
        {
            Opcode = _Opcode;
        }
    }
}
