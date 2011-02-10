using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.Game;
using Sienna.Intercom;
using Sienna.Database;

using System.Security.Cryptography;

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
        public static bool CreateAccount(string name, string password)
        {
            name = LogonMgr.LDatabase.EscapeString(name);
            password = LogonMgr.LDatabase.EscapeString(password);

            List<Row> accountExists = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE `username` = '" + name + "'");
            
            if (accountExists.Count > 0)
                return false;

            var data = Encoding.ASCII.GetBytes(password);
            var hashd = new SHA1Managed().ComputeHash(data);

            var hash = string.Empty;

            foreach (var b in hashd)
                hash += b.ToString("X2");

            password = hash.ToUpper();

            LogonMgr.LDatabase.Execute("INSERT INTO accounts (`username`, `sha_password`, `sessionkey`) VALUES ('" + name + "', '" + password + "', '1234')");

            return true;
        }
        public static bool ChangePassword(string name, string password)
        {
            name = LogonMgr.LDatabase.EscapeString(name);
            password = LogonMgr.LDatabase.EscapeString(password);

            List<Row> accountExists = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE `username` = '" + name + "'");

            if (accountExists.Count < 1)
                return false;

            var data = Encoding.ASCII.GetBytes(password);
            var hashd = new SHA1Managed().ComputeHash(data);

            var hash = string.Empty;

            foreach (var b in hashd)
                hash += b.ToString("X2");

            password = hash.ToUpper();

            LogonMgr.LDatabase.Execute("UPDATE accounts SET `sha_password` = '" + password + "' WHERE `username` = '" + name + "'");

            return true;
        }
    }
}
