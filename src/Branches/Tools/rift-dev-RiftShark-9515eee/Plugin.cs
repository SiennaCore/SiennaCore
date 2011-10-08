using System;
using System.Collections.Generic;
using System.Text;

namespace RiftShark
{
    public abstract class Plugin
    {
        public Plugin() { }

        protected internal abstract void OnPacket(RiftPacket pPacket);
    }
}
