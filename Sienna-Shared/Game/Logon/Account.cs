using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    [Serializable]
    public class Account
    {
        public Account(string User, string ShaPassword)
        {
            Username = User;
            Hash = ShaPassword;
        }

        public string Username;
        public string Hash;
    }
}
