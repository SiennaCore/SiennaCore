using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public abstract class ISerializablePacket
    {
        public abstract void OnRead(RiftClient From);

        public long GetOpcode()
        {
            ISerializableAttribute[] Atrib = GetType().GetCustomAttributes(typeof(ISerializableAttribute), true) as ISerializableAttribute[];

            if (Atrib != null && Atrib.Length > 0)
                return Atrib[0].GetOpcode();
            else
                return 0;
        }
    }
}
