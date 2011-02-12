using System;

namespace Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ReadOnly : Attribute
    {
        public ReadOnly()
        {
        }
    }
}