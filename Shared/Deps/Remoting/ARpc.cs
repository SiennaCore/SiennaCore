using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public abstract class ARpc : MarshalByRefObject
    {
        /// <summary>
        /// Function Called when client doesn"t response to ping
        /// </summary>
        /// <param name="Id">Client RpcId</param>
        public virtual void Disconnected(int Id)
        {

        }
    }
}
