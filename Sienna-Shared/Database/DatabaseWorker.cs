using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace Sienna
{
    public class AsyncEventInfo
    {
        public DatabaseWorker.Method meth;
        public bool treatement;
    }

    public class DatabaseWorker
    {
        public delegate void Method();

        public static List<AsyncEventInfo> AsyncEvents = new List<AsyncEventInfo>();

        public static void BeginInvoke(Method method)
        {
            AsyncEventInfo pck = new AsyncEventInfo();
            pck.meth = method;
            pck.treatement = false;

            AsyncEvents.Add(pck);
        }

        public static void StartWorkers(int Count)
        {
            for (int i = 0; i < Count; i++)
            {
                Thread t = new Thread(new ThreadStart(DatabaseThread));
                t.Start();
            }
        }

        public static void DatabaseThread()
        {
            while (true)
            {
                AsyncEventInfo[] Evts = AsyncEvents.ToArray();

                foreach (AsyncEventInfo evt in Evts)
                {
                    lock (evt)
                    {
                        if (evt.treatement)
                            continue;

                        evt.treatement = true;
                    }

                    evt.meth.Invoke();

                    AsyncEvents.Remove(evt);
                }

                Thread.Sleep(30);
            }
        }
    }
}
