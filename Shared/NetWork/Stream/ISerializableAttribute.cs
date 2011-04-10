using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class ISerializableAttribute : Attribute
    {
        protected long Opcode;

        public ISerializableAttribute(long Opcode)
        {
            this.Opcode = Opcode;
        }

        public long GetOpcode()
        {
            return Opcode;
        }
    }
}
