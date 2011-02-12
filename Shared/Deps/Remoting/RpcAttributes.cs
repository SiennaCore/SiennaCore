using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcAttributes : Attribute
    {
        string[] _Autorised;

        public RpcAttributes(string[] Autorised)
        {
            _Autorised = Autorised;
        }

        public string[] Authorised
        { get { return _Autorised; } }
    }
}
