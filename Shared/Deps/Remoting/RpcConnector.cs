using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Shared
{
    public class RpcConnector : ARpc
    {
        static public RpcServer _Server;
        private RpcServer PServer;

        public RpcConnector(RpcServer Server)
        {
            Log.Info("RpcConnector", "Server = " + Server);
            PServer = Server;
        }

        public RpcConnector()
        {
            PServer = _Server;
        }

        public int Connect(string Name)
        {
            if (PServer != null)
                return PServer.GenerateID(Name);
            else
                return 0;
        }

        public void Ping(int Id)
        {
            if (PServer != null)
                PServer.UpdatePing(Id);
        }
    }
}