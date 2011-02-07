using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    [Serializable]
    public class Account
    {
        public Account(long AccountID, string User, string SKey, string ShaPassword, int GM, string Mail)
        {
            ID = AccountID;
            Username = User;
            Hash = ShaPassword;
            SessionKey = SKey;
            Email = Mail;
            GMLevel = GM;
        }

        public long ID;
        public string Username;
        public string Hash;
        public string SessionKey;
        public string Email;
        public int GMLevel;
    }
}
