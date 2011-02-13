using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "RandomNames", PreCache = true)]
[Serializable]
public class RandomName : DataObject
{
    [DataElement(Unique=true,Varchar=9)]
    public string Name;
}