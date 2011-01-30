using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Sienna.Intercom
{
    public class IntercomServer
    {
        protected TcpChannel _Channel;
        protected int _Port;

        public IntercomServer()
        {
            
        }

        protected int RegisterServices(string Key)
        {
            int ClassBinded = 0;

            foreach (Assembly Asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in Asm.GetTypes())
                {
                    if (t.BaseType != null && t.BaseType.Name.Contains("RemoteObject"))
                    {
                        WellKnownServiceTypeEntry wkst = new WellKnownServiceTypeEntry(t, Key + "/" + t.Name, WellKnownObjectMode.SingleCall);
                        RemotingConfiguration.RegisterWellKnownServiceType(wkst);
                        ClassBinded++;
                    }
                }
            }

            return ClassBinded;
        }

        public int Bind(int Port, string Key)
        {
            try
            {
                _Port = Port;
                _Channel = new TcpChannel(_Port);
                ChannelServices.RegisterChannel(_Channel, false);

                int ClassBinded = RegisterServices(Key);

                return ClassBinded;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " " + e.Source + " " + e.StackTrace);
                return -1;
            }
        }
    }
}
