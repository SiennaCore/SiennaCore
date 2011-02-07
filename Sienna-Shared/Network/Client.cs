using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Sienna.Network
{
    public abstract class Client
    {
        protected Socket _Socket;

        public Client()
        {

        }

        /// <summary>
        /// Constructor called from server, must be used only for client initialization from server
        /// </summary>
        /// <param name="Sock">Client Socket</param>
        public Client(Socket Sock)
        {
            _Socket = Sock;
        }

        /// <summary>
        /// Send an array of bytes to client
        /// </summary>
        /// <param name="Buffer">Array to send</param>
        public void Send(byte[] Buffer)
        {
            _Socket.Send(Buffer);
        }

        /// <summary>
        /// Disconnect client from server
        /// </summary>
        public void Disconnect()
        {
            _Socket.Close();
        }
    }
}
