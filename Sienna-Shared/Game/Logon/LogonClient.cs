using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

using Sienna.Network;

namespace Sienna.Game
{
    public class LogonClient : Client
    {
        // Base Constructors from abstract server
        public LogonClient() : base() { }
        public LogonClient(Socket Sock) : base(Sock) { }

        public void Send(UInt16 Opcode, PacketStream ps)
        {
            byte[] data = ps.ToLogonPacket(Opcode);
            Log.PacketDump(data);

            _Socket.Send(data);
        }
    }
}
