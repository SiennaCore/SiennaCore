using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

namespace Shared
{
    [RpcAttributes(new string[] { "CharacterServer" })]
    public class AccountMgr : ARpc
    {
        static public ObjectDatabase AccountDB = null;

        public Account GetAccountBySession(string SessionKey)
        {
            return AccountDB.SelectObject<Account>("SessionKey='" + AccountDB.Escape(SessionKey).ToUpper() + "'");
        }

        public Account GetAccountByPassword(string Username, string Sha_Password)
        {
            return AccountDB.SelectObject<Account>("Username='" + AccountDB.Escape(Username).ToUpper() + "' AND Sha_Password='" + AccountDB.Escape(Sha_Password).ToUpper() + "'");
        }

        public Account GetAccount(string Username)
        {
            return AccountDB.SelectObject<Account>("SessionKey='" + AccountDB.Escape(Username).ToUpper() + "'");
        }

        public Account GetAccount(int AccountID)
        {
            return AccountDB.SelectObject<Account>("Id='" + AccountID + "'");
        }
    }
}
