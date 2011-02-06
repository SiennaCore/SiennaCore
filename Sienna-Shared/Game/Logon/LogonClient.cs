using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Net.Sockets;

using Sienna.Network;
using Sienna.zlib;

namespace Sienna.Game
{
    public class LogonClient : Client
    {
        // Base Constructors from abstract server
        public LogonClient() : base() { }

        public LogonClient(Socket Sock) : base(Sock)
        {
            DataStr = new MemoryStream();
            ZStream = new ZOutputStream(DataStr, zlibConst.Z_DEFAULT_COMPRESSION);
            ZStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
        }

        public bool ClientCompressPackets = false;
        public bool ServerCompressPackets = false;

        protected MemoryStream DataStr;
        protected ZOutputStream ZStream;

        protected byte[] Deflate(byte[] Input)
        {
            ZStream.Write(Input, 0, Input.Length);
            ZStream.Flush();

            byte[] DeflatedData = DataStr.ToArray();
            DataStr.SetLength(0);
            return DeflatedData;
        }

        public void Send(LogonOpcodes Opcode, PacketStream ps)
        {
            byte[] data = null;

            if (ServerCompressPackets)
            {
                data = Deflate(ps.ToLogonPacket((ushort)Opcode));
                //Log.PacketDump(data);
            }
            else
                data = ps.ToLogonPacket((ushort)Opcode);

            _Socket.Send(data);
        }
    }
}
