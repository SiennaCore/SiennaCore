using System;

namespace Shared.Database
{
    // Doit être utilisé en atttibut
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DataElement : Attribute
    {
        public DataElement()
        {
            AllowDbNull = true;
            Unique = false;
            Index = false;
            Varchar = 0;
            Decimal = 10;
        }

        // Indique si la var peut être null
        public bool AllowDbNull { get; set; }

        // Indique si la var est unique
        public bool Unique { get; set; }

        // Indique si c'est un Index
        public bool Index { get; set; }

        // Indique la taille du varchar , 0 = TEXT
        public byte Varchar { get; set; }

        // Indique le nombre de chiffres après la virgule
        public byte Decimal { get; set; }
    }
}