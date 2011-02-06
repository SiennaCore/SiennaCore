using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using System.Net;
using System.Net.Sockets;

namespace Sienna.Network
{
    /// <summary>
    /// Determine socket states for thread pool
    /// </summary>
    public class SocketState
    {
        public SocketState(Socket Client)
        {
            Locked = false;
            Socket = Client;
        }

        public bool Locked;
        public Socket Socket;
    }

    /// <summary>
    /// Generic TCP server class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Server<T>
    {
        // Members
        private int _Port;
        private int _Threads;
        private int _UpdateTime;
        private bool _IsRunning;
        private Socket _Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Dictionary<T, SocketState> _Clients = new Dictionary<T, SocketState>();

        // Events
        protected abstract void OnConnect(T Client);
        protected abstract void OnDisconnect(T Client);
        protected abstract void OnRead(T Client, byte[] ReadenBytes);

        /// <summary>
        /// Initialize instance of Server
        /// </summary>
        /// <param name="ThreadCount">Number of Net I/O Threads</param>
        /// <param name="UpdateTime">Update Interval of I/O Threads</param>
        public Server(int ThreadCount, int UpdateTime)
        {
            _Threads = ThreadCount;
            _UpdateTime = UpdateTime;
        }

        /// <summary>
        /// Bind server to port and begin accept clients
        /// </summary>
        /// <param name="Port">Server port</param>
        /// <returns>Return true if bind is ok</returns>
        public bool Bind(int Port)
        {
            _Port = Port;

            try
            {
                _Server.Bind(new IPEndPoint(IPAddress.Any, Port));
                _Server.Listen(-1);
                _Server.BeginAccept(_Callback_OnAccept, this);
                _IsRunning = true;

                for (int i = 0; i < _Threads; i++)
                {
                    Thread t = new Thread(new ThreadStart(_ThreadUpdate));
                    t.Start();
                }
            }
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// I/O Threads routine
        /// </summary>
        private void _ThreadUpdate()
        {
            while (_IsRunning)
            {
                foreach (KeyValuePair<T, SocketState> Client in _Clients.ToArray())
                {
                    try
                    {
                        if (!Client.Value.Locked)
                        {
                            Client.Value.Locked = true;

                            Socket ClientSocket = Client.Value.Socket;

                            // Check if client is disconnected
                            if (!ClientSocket.Connected || (ClientSocket.Poll(0, SelectMode.SelectRead) && ClientSocket.Available == 0))
                            {
                                OnDisconnect(Client.Key);
                                ClientSocket.Close();
                                _Clients.Remove(Client.Key);
                                continue;
                            }

                            // Check if there is any data in the buffer
                            if (ClientSocket.Available != 0)
                            {
                                // Get incoming bytes
                                Byte[] Buffer = new Byte[ClientSocket.Available];
                                int ReadenBytes = ClientSocket.Receive(Buffer, Buffer.Length, SocketFlags.None);

                                OnRead(Client.Key, Buffer);
                            }

                            Client.Value.Locked = false;
                        }
                    }
                    catch (Exception) { }
                }

                Thread.Sleep(_UpdateTime);
            }
        }

        /// <summary>
        /// Called when someone begin a connection
        /// </summary>
        /// <param name="res"></param>
        private void _Callback_OnAccept(IAsyncResult res)
        {
            Socket ClientSock = _Server.EndAccept(res);
            _Server.BeginAccept(_Callback_OnAccept, this);

            T Client = (T)Activator.CreateInstance(typeof(T), new Object[] { ClientSock });
            _Clients.Add(Client, new SocketState(ClientSock));
            OnConnect(Client); 
        }
    }
}
