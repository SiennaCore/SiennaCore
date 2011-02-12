using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public abstract class aConfig
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class aConfigAttributes : Attribute
    {
        protected string _FileName;

        public aConfigAttributes(string FileName)
        {
            _FileName = FileName;
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
        }
    }
}
