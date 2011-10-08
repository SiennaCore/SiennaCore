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

namespace Common
{
    [Rpc(false, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 2)]
    public class MapMgr : RpcObject
    {
        static public MapServerInfo MapInfo;
        static public RpcClient Client;

        public override void OnServerConnected()
        {
            if (MapInfo != null)
                Client.GetServerObject<WorldMgr>().RegisterMaps(MapInfo, Client.Info);
        }

        public Dictionary<string, long> Connecting = new Dictionary<string, long>();

        public Realm GetRealm()
        {
            return Client.GetServerObject<CharactersMgr>().GetRealm();
        }

        public Account GetAccount(string Email)
        {
            return GetRealm().GetObject<AccountMgr>().GetAccountByEmail(Email);
        }

        public void RegisterConnecting(string Email, long CharacterID)
        {
            Email = Email.ToLower();

            Log.Success("MapMgr", "Registering email : " + Email + ", Character = " + CharacterID);

            if (Connecting.ContainsKey(Email))
                Connecting[Email] = CharacterID;
            else
                Connecting.Add(Email, CharacterID);
        }

        public long GetConnecting(string Email)
        {
            Email = Email.ToLower();

            Log.Success("MapMgr", "GetConnecting : " + Email);

            if (Connecting.ContainsKey(Email))
                return Connecting[Email];
            return 0;
        }
    }
}
