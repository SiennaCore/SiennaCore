using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels.Tcp;

namespace Shared
{
    public class ClientInfo
    {
        static public long MaxTime = 4000;
        public Stopwatch _Watch = new Stopwatch();
        public RpcServer _Server;
        public string _Name;
        public int _Id;
        public long _Elapsed;

        public ClientInfo(RpcServer Server, string Name, int Id)
        {
            _Server = Server;
            _Name = Name;
            _Id = Id;
            Load();

            _Watch.Start();
        }

        public bool Update()
        {
            if (_Watch.ElapsedMilliseconds > MaxTime)
                return true;
            else 
                return false;
        }

        public void Reset()
        {
            _Watch.Stop();
            _Watch.Reset();
            _Watch.Start();
        }

        public void Load()
        {
            foreach (ARpc Rp in _Server._Rpcs)
            {
                Log.Info("ClientInfo", "Loading of : " + Rp.GetType());
                RemotingConfiguration.RegisterWellKnownServiceType(Rp.GetType(), Rp.GetType().Name + _Id + _Server._Key, WellKnownObjectMode.Singleton);
            }
        }
    }
}
