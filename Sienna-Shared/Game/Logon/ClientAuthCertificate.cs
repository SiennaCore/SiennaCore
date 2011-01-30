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

        public bool IsValid()
        {
            Username = LogonMgr.LDatabase.EscapeString(Username);
            Hash = LogonMgr.LDatabase.EscapeString(Hash);

            List<Row> Result = LogonMgr.LDatabase.Execute("SELECT * FROM accounts WHERE username = \"" + Username + "\" AND sha_password = \"" + Hash + "\"");

            if (Result.Count == 0)
                return false;

            return true;
        }
    }
}
