using System;

namespace Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrimaryKey : Attribute
    {
        public PrimaryKey()
        {
            AutoIncrement = false;
            IncrementValue = 0;
        }

        // Indique si c'est de l'auto incrément
		// Shows if the auto increment
        public bool AutoIncrement { get; set; }

        public int IncrementValue { get; set; }
    }
}
