using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.Network;
using Sienna.zlib;

using System.Reflection;
using System.IO;

namespace Sienna.Game
{
    public class LogonServer : Server<LogonClient>
    {
        private delegate void LogonPacketHandler(LogonClient From, PacketStream Data);
        private Dictionary<UInt16, LogonPacketHandler> Handlers = new Dictionary<ushort,LogonPacketHandler>();

        // Base Constructor from abstract server
        public LogonServer(int ThreadCount, int UpdateTime) : base(ThreadCount, UpdateTime)
        {
            Type CurrentType = GetType();
            Assembly CurrentAssembly = CurrentType.Assembly;

            foreach (Type t in CurrentAssembly.GetTypes())
                foreach (MethodInfo m in t.GetMethods())
                    foreach (object at in m.GetCustomAttributes(typeof(LogonPacketAttribute), false))
                    {
                        LogonPacketAttribute attr = at as LogonPacketAttribute;
                        LogonPacketHandler handler = (LogonPacketHandler)Delegate.CreateDelegate(typeof(LogonPacketHandler), m);

                        Handlers.Add(attr.Opcode, handler);
                    }
        }

        protected override void OnConnect(LogonClient Client)
        {

        }

        protected override void OnDisconnect(LogonClient Client)
        {

        }

        protected override void OnRead(LogonClient Client, byte[] ReadenBytes)
        {
            PacketStream Ps = new PacketStream(ReadenBytes);
            Ps.Rewind(0);

            //int offset = 0;

            while (!Ps.Empty())
            {
                try
                {
                    /*if (Client.ClientCompressPackets)
                    {
                        int StartPos = offset;
                        int EndPos = -1;
                        for (int i = offset; i < ReadenBytes.Length; i++)
                        {
                            if (ReadenBytes[i] == 0x00 && ReadenBytes.Length - i >= 4)
                            {
                                if (ExtendedBitConverter.ConvertToUInt32(ReadenBytes[i + 1], ReadenBytes[i + 2], ReadenBytes[i + 3], ReadenBytes[i + 4]) == 0xFF)
                                {
                                    EndPos = i;
                                    offset = i + 5;
                                    break;
                                }
                            }
                        }

                        // Read error
                        if (EndPos == -1)
                        {
                            Log.Error("Client sent an invalid deflated packet");
                            return;
                        }

                        byte[] pck = new byte[EndPos - StartPos];
                        Array.Copy(ReadenBytes, StartPos, pck, 0, pck.Length);

                        pck = ZlibMgr.Decompress(pck);
                    }
                    else
                    {*/
                        UInt64 Size = (byte)Ps.ReadByte();

                        if (Size == 0)
                            return;

                        bool isLongPacket = Size >= 0x80;

                        if (isLongPacket)
                        {
                            UInt64 BinaryOffset = (byte)Ps.ReadByte();
                            Size = Size + ((BinaryOffset - 1) * Size);
                        }

                        UInt16 Opcode = Ps.GetUInt16();

                        //Log.Error("CLIENT_SENDED_OPCODE : " + Opcode);

                        byte[] Buffer = Ps.Read((int)(Size - (ulong)sizeof(UInt16)));
                        PacketStream ps = new PacketStream(Buffer);
                        ps.Rewind(0);

                        if (Handlers.ContainsKey(Opcode))
                            Handlers[Opcode].Invoke(Client, ps);
                   //}
                }
                catch (Exception e) { Log.Error(e.Message + " " + e.Source + " " + e.StackTrace); }
            }
        }
    }
}
