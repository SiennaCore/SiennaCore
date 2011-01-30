using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.Network;

using System.Reflection;

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

            while (!Ps.Empty())
            {
                try
                {
                    byte Size = (byte)Ps.ReadByte();
                    UInt16 Opcode = Ps.GetUInt16();
                    byte[] Buffer = Ps.Read(Size - sizeof(UInt16));

                    if (Handlers.ContainsKey(Opcode))
                        Handlers[Opcode].Invoke(Client, new PacketStream(Buffer));
                }
                catch (Exception e) { Log.Error(e.Message + " " + e.Source + " " + e.StackTrace); }
            }
        }
    }
}
