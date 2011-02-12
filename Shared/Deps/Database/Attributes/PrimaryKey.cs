using System;

namespace Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrimaryKey : Attribute
    {
        public PrimaryKey()
        {
            AutoIncrement = false;
        }

        // Indique si c'est de l'auto incrément
        public bool AutoIncrement { get; set; }
    }
}
