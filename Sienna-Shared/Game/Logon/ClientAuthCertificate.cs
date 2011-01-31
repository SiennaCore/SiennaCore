using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sienna.Database;

namespace Sienna.Game
{
    public class ClientAuthCertificate
    {
        public string Username;
        public string Hash;
        public string Sessionkey;

        /// <summary>
        /// Check client authentification certificate
        /// </summary>
        /// <param name="UsingCertificateServer">If implementing a custom certificate server for advanced authentification set this to true</param>
        /// <returns></returns>
        public bool IsValid(bool UsingCertificateServer)
        {
            Username = LogonMgr.LDatabase.EscapeString(Username);
            Hash = LogonMgr.LDatabase.EscapeString(Hash);
            Sessionkey = LogonMgr.LDatabase.EscapeString(Sessionkey);

            List<Row> Result = null;

            if (!UsingCertificateServer)
                Result = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE username = \"" + Username + "\" AND sha_password = \"" + Hash + "\"");
            else
                Result = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE sessionkey = \"" + Sessionkey + "\"");

            if (Result.Count == 0)
                return false;

            List<Row> IsBanned = LogonMgr.LDatabase.Execute("SELECT * FROM accounts_banned WHERE id = \"" + Result[0]["id"] + "\"");
            bool IsAccountBanned = false;

            // Check if there is an active ban
            foreach (Row r in IsBanned)
                if (r["banend"] == r["banstart"] || long.Parse(r["banend"]) < DateTime.Now.Ticks)
                    IsAccountBanned = true;

            return IsAccountBanned;
        }
    }
}
