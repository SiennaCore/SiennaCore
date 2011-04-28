using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels.Tcp;


namespace Shared
{
    public class RpcClient
    {
        #region Manager

        static private TcpChannel _Channel
        {
            get
            {
                return RpcServer._Channel;
            }
            set
            {
                RpcServer._Channel = value;
            }
        }

        static private Dictionary<string, RpcClient> _Rc = new Dictionary<string, RpcClient>();
        static public bool InitRpcClient(string Name, string Key, string Ip, int Port)
        {
            if (_Rc.ContainsKey(Name))
                return false;

            RpcClient Client = new RpcClient(Name, Ip, Port, Key);

            if (Client.Connect())
                _Rc.Add(Name, Client);
            else 
                return false;

            return true;
        }
        static public int GetRpcClientId(string Name)
        {
            if (_Rc.ContainsKey(Name))
                return _Rc[Name]._Id;

            return 0;
        }

        #endregion

        private string _Ip;
        private int _Port;
        private string _Key;
        private string _Name;
        public int _Id = 0;
        private Timer _Ping;
        private RpcConnector Connector;

        public RpcClient(string Name, string Ip, int port, string Key)
        {
            _Ip = Ip;
            _Port = port;
            _Key = Key;
            _Name = Name;
        }

        public bool Connect(int attempt)
        {
            if (attempt <= 0)
                attempt = 1;

            while (attempt > 0 && !Connect())
            {
                --attempt;
                Log.Debug("RpcClient", "Attempt : " + attempt);
                System.Threading.Thread.Sleep(1000);
            }

            return attempt > 0;
        }

        public bool Connect()
        {
            try
            {
                WellKnownClientTypeEntry CoRemote =
                new WellKnownClientTypeEntry(typeof(RpcConnector), "tcp://" + _Ip + ":" + _Port + "/" + typeof(RpcConnector).Name + _Key);
        
                if (_Channel == null)
                {
                    _Channel = new TcpChannel();
                    ChannelServices.RegisterChannel(_Channel, false);
                    RemotingConfiguration.RegisterWellKnownClientType(CoRemote);
                }

                Log.Info("Connect", "Registering connector : " + CoRemote.ObjectUrl);

                Connector = (RpcConnector)Activator.GetObject(CoRemote.ObjectType, CoRemote.ObjectUrl);
                _Id = Connector.Connect(_Name);

                Log.Success("RpcClient", "Connected to : " + _Ip + ":" + _Port);
                Load();
            }
            catch (Exception e)
            {
                Log.Error("RcpClient", "Can not connect to : " + _Ip + ":" + _Port);
                Log.Error("RpcClient", "Erreur = " + e.ToString());
                try
                {
                    ChannelServices.UnregisterChannel(_Channel);
                }
                catch (Exception) { }

                _Channel = null;
                return false;
            }

            try
            {
                _Ping = new Timer();
                _Ping.Interval = 1000;
                _Ping.Elapsed += Ping;
                _Ping.Start();
            }
            catch (Exception e)
            {
                Log.Error("RpcClient", "Erreur :" + e.ToString());
            }

            return true;
        }

        public void Load()
        {
            Log.Info("RpcClient", "Loading");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if (!type.IsSubclassOf(typeof(ARpc)))
                        continue;

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
                            Log.Debug("RpcClient", "Registering class : " + type.Name);

                            WellKnownClientTypeEntry remotetype = new WellKnownClientTypeEntry(type,
                                "tcp://" + _Ip + ":" + _Port + "/" + type.Name + _Id + _Key);
                            RemotingConfiguration.RegisterWellKnownClientType(remotetype);
                            break;
                        }
                    }
                }
            }
          
        }

        public void Ping(Object obj, EventArgs Arg)
        {
            try
            {
                _Ping.Stop();
                Connector.Ping(_Id);
                _Ping.Start();
            }
            catch (Exception)
            {
                Log.Error("RpcClient", "Server disconnected : " + _Ip + ":" + _Port);
                Log.Info("RpcClient", "Attempting to reconnect : 100");
                if (Connect(100))
                    _Ping.Start();
            }
        }
    }
}
