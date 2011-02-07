using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.Game;
using Sienna.Intercom;
using Sienna.Database;

namespace Sienna
{
    public class AccountManager : RemoteObject<AccountManager>
    {
        protected static Dictionary<String, Account> Accounts = new Dictionary<string, Account>();

        public AccountManager(string Url, string Key) : base(Url, Key) { }

        public Account GetAccountByUsername(string Name)
        {
            if (Accounts.ContainsKey(Name))
                return Accounts[Name];

            Name = LogonMgr.LDatabase.EscapeString(Name);
            List<Row> Result = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE username = \"" + Name + "\"");

            if (Result.Count == 0)
                return null;

            Row row = Result[0];
            Account acct = new Account(long.Parse(row["id"]), row["username"], row["sessionkey"], row["sha_password"], int.Parse(row["gmlevel"]), row["email"]);
            Accounts.Add(Name, acct);

            return acct;
        }
    }
}
