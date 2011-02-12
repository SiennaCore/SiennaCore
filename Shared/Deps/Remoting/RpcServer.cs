using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels.Tcp;
using System.Timers;

namespace Shared
{
    public class RpcServer
    {
        #region Manager

        static private Dictionary<int, RpcServer> _Rs = new Dictionary<int, RpcServer>();
        static public bool InitRpcServer(string Name, string Key, int Port)
        {
            if (_Rs.ContainsKey(Port))
                return false;

            RpcServer Server = new RpcServer(Name, Key, Port);

            if (Server.Listen())
                _Rs.Add(Port, Server);
            else 
                return false;

            return true;
        }

        #endregion

        private TcpChannel _Channel;
        private string _Name;
        public string _Key;
        private int _Port;
        private int _ClientId = 0;
        private Timer _TimeOut;
        static public int TimeOutTime = 2000;
        private RpcConnector Connector;

        private Dictionary<int, ClientInfo> _Clients = new Dictionary<int, ClientInfo>();
        public List<ARpc> _Rpcs = new List<ARpc>();

        public RpcServer(string Name, string Key, int port)
        {
            _Name = Name;
            _Key = Key;
            _Port = port;

            _TimeOut = new Timer();
            _TimeOut.Interval = TimeOutTime;
            _TimeOut.Elapsed += CheckTimeOut;
            _TimeOut.Start();
        }

        public bool Listen()
        {
            if (_Channel != null)
                return false;

            try
            {
                Log.Info("RpcServer", "Starting... " + _Port);
                _Channel = new TcpChannel(_Port);
                ChannelServices.RegisterChannel(_Channel, false);
                Load();
            }
            catch (Exception e)
            {
                Log.Error("RpcServer", "Can not start RPC server : " + e.ToString());
                return false;
            }

            Log.Succes("RpcServer", "Server listening on : " + _Port);
            return true;
        }

        public void Load()
        {
            Log.Info("Load", "Registering RpcConnector : " + typeof(RpcConnector).Name + _Key);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RpcConnector), typeof(RpcConnector).Name + _Key, WellKnownObjectMode.SingleCall);
            Connector = (RpcConnector)Activator.CreateInstance(typeof(RpcConnector), this);
            RpcConnector._Server = this;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if (type.IsSubclassOf(typeof(ARpc)))
                    {
                        object[] attrib = type.GetCustomAttributes(typeof(RpcAttributes), true);
                        if (attrib.Length <= 0)
                            continue;

                        RpcAttributes[] packethandlerattribs =
                            (RpcAttributes[])type.GetCustomAttributes(typeof(RpcAttributes), true);

                        if (packethandlerattribs.Length <= 0)
                            continue;

                        foreach (string auth in packethandlerattribs[0].Authorised)
                        {
                            if (auth.StartsWith(_Name))
                            {
                                Log.Info("RpcServer", "Registering Rpc : " + type.Name);

                                RemotingConfiguration.RegisterWellKnownServiceType(type, type.Name + _Key, WellKnownObjectMode.Singleton);

                                ARpc Rpc = (ARpc)Activator.CreateInstance(type);
                                _Rpcs.Add(Rpc);
                                break;
                            }
                        }
                    }

                }
            }
        }

        public void CheckTimeOut(object Obj, EventArgs Args)
        {
            lock (_Clients)
            {
                List<ClientInfo> Infos = new List<ClientInfo>();

                foreach(ClientInfo Info in _Clients.Values)
                    if (Info.Update())
                        Infos.Add(Info);

                foreach (ClientInfo Info in Infos)
                {
                    Log.Error("RpcServer", "Client disconnected : " + Info._Id);

                    _Clients.Remove(Info._Id);

                    foreach (ARpc Rp in _Rpcs)
                        Rp.Disconnected(Info._Id);
                }
            }
        }


        #region RPCCONNECTOR

        public int GenerateID(string Name)
        {
            lock (_Clients)
            {
                ++_ClientId;
                
                Log.Info("RpcServer", "New client: " + Name + ", ID=" + _ClientId);
               
                _Clients.Add(_ClientId, new ClientInfo(this,Name,_ClientId) );
            }

            return _ClientId;
        }

        public void UpdatePing(int Id)
        {
            lock (_Clients)
            {
                if (!_Clients.ContainsKey(Id))
                    return;

                _Clients[Id].Reset();
            }
        }

        #endregion
    }
}
