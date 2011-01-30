using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna.Intercom
{
    public class RemoteObject<T> : MarshalByRefObject
    {
        protected string _Url;
        protected T RemoteObjectInstance;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public RemoteObject(string Url, string Key)
        {
            _Url = Url;

            if (Url != "")
                RemoteObjectInstance = (T)Activator.GetObject(typeof(T), "tcp://" + _Url + "/" + Key + "/" + typeof(T).Name);
            else
                RemoteObjectInstance = (T)Activator.CreateInstance(typeof(T));
        }

        public T get
        {
            get
            {
                return RemoteObjectInstance;
            }
        }
    }
}
