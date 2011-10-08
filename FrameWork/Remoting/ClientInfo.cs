/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    [Serializable]
    public class RpcClientInfo
    {
        public string Name;
        public int RpcID;

        public string Ip;
        public int Port;

        public RpcClientInfo(string Name, string Ip, int Port, int RpcID)
        {
            this.Name = Name;
            this.Ip = Ip;
            this.Port = Port;
            this.RpcID = RpcID;
        }

        public string Description()
        {
            return "[" + RpcID + "]\t| " + Name + "| " + Ip + ":" + Port;
        }

        public bool Connected = true;
    }
}
