using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;
using Shared.NetWork;

namespace Shared.NetWork
{
    public class RiftServer  :TCPManager
    {
        public RiftServer()
            : base()
        {
            PacketOut.OpcodeInLen = true;
            PacketOut.OpcodeReverse = true;
            PacketOut.SizeInLen = false;
            PacketOut.Struct = PackStruct.SizeAndOpcode;
            PacketOut.SizeLen = 0;
        }

        protected override BaseClient GetNewClient()
        {
            return new RiftClient(this);
        }
    }
}
