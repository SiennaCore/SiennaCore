using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterCommands : Attribute
    {
        private string _command;
        private string _method;
        private UInt16 _minargs;

        public RegisterCommands(string command, string method, UInt16 minargs)
        {
            _command = command;
            _method = method;
            _minargs = minargs;
        }
        public string Command
        {
            get { return _command; }
        }
        public string Method
        {
            get { return _method; }
        }
        public UInt16 Minargs
        {
            get { return _minargs; }
        }
    }
}
