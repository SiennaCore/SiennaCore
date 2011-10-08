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

using FrameWork;

namespace RealmServer
{
    [aConfigAttributes("Configs/RealmServer.xml")]
    public class RealmConfig : aConfig
    {
        public RpcClientConfig RpcCharacter = new RpcClientConfig("127.0.0.1","127.0.0.1",6800);
        public RpcServerConfig RpcMapServer = new RpcServerConfig("127.0.0.1",6499,6500);

        public Realm RealmInfo = new Realm();

        public DatabaseInfo CharactersDB = new DatabaseInfo();
        public DatabaseInfo WorldDB = new DatabaseInfo();
        public LogInfo LogLevel = new LogInfo();
    }
}
