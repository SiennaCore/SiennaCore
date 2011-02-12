using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.NetWork
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CryptAttribute : Attribute
    {
        public string _CryptName;

        public CryptAttribute(string CryptName)
        {
            _CryptName = CryptName;
        }
    }
}
