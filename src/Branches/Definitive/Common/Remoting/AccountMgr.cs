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
    [Rpc(true,System.Runtime.Remoting.WellKnownObjectMode.Singleton,1)]
    public class AccountMgr : RpcObject
    {
        static public MySQLObjectDatabase AccountDB;

        #region Accounts

        public Account GetAccount(long Id)
        {
            return AccountDB.SelectObject<Account>("Id=" + Id);
        }

        public Account GetAccount(string SessionKey)
        {
            return AccountDB.SelectObject<Account>("SessionKey='" + AccountDB.Escape(SessionKey) + "'");
        }

        public Account GetAccountByUsername(string Username)
        {
            return AccountDB.SelectObject<Account>("Username = '" + AccountDB.Escape(Username) + "'");
        }

        public Account GetAccountByEmail(string Email)
        {
            return AccountDB.SelectObject<Account>("Email='" + AccountDB.Escape(Email) + "'");
        }

        public Account GetAccount(string Username, string Sha_Password)
        {
            return AccountDB.SelectObject<Account>("Username = '" + AccountDB.Escape(Username) + "' AND Sha_Password='" + AccountDB.Escape(Sha_Password) + "'");
        }

        #endregion

        #region Realms

        private List<Realm> Realms = new List<Realm>();

        public Realm GetRealm(byte RealmId)
        {
            return Realms.Find(info => info.RealmId == RealmId);
        }

        public Realm GetRealm(int RiftId)
        {
            return Realms.Find(info => info.RiftId == RiftId);
        }

        public Realm[] GetRealms()
        {
            return Realms.ToArray();
        }

        public bool RegisterRealm(Realm Rm,RpcClientInfo Info)
        {
            Log.Debug("AccountMgr", "Realm Registering : " + Rm.Name);

            if (Rm == null)
                return false;

            Rm.GenerateName();

            Realm Already = GetRealm(Rm.RealmId);
            if (Already == null)
            {
                Already = AccountDB.SelectObject<Realm>("RealmId=" + Rm.RealmId);
                if (Already == null)
                    AccountDB.AddObject(Rm);
            }

            if (Already != null)
            {
                Rm.ObjectId = Already.ObjectId;
                Realms.Remove(Already);
            }

            Rm.RpcInfo = Info;
            Rm.Dirty = true;

            AccountDB.SaveObject(Rm);
            Realms.Add(Rm);

            if (Info != null)
            {
                Log.Success("AccountMgr", "Realm Online : " + Rm.Name);
            }
            else
                Log.Success("AccountMgr", "Realm loaded : " + Rm.Name);

            return true;
        }

        public void LoadRealms()
        {
            Realm[] Rms = AccountDB.SelectAllObjects<Realm>().ToArray();
            foreach (Realm Rm in Rms)
                RegisterRealm(Rm, null);
        }

        public override void OnClientDisconnected(RpcClientInfo Info)
        {
            foreach (Realm Rm in GetRealms())
                if (Rm.RpcInfo != null && Rm.RpcInfo.RpcID == Info.RpcID)
                {
                    Log.Notice("AccountMgr", "Realm Offline : " + Rm.Name);
                    Rm.RpcInfo = null;
                }
        }

        #endregion
    }
}
