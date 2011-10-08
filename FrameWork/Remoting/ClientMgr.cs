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
using System.Timers;

namespace FrameWork
{
    [RpcAttribute(false, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 0)]
    public class ClientMgr : RpcObject
    {
        public override void OnClientConnected(RpcClientInfo Info)
        {
            Log.Notice("ClientMgr", Info.Description() + " | Connected");
        }

        public override void OnClientDisconnected(RpcClientInfo Info)
        {
            Log.Notice("ClientMgr", Info.Description() + " | Disconnected");
        }

        public override void OnServerConnected()
        {
            Log.Notice("ClientMgr", "Server connected !");
        }

        public override void OnServerDisconnected()
        {
            Log.Notice("ClientMgr", "Server disconnected !");

        }

        public void Ping()
        {

        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
