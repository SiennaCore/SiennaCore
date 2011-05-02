using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Items_Template", PreCache = true)]
[Serializable]
public class Items_Template : DataObject
{
    [DataElement]
    public long Id;

    [DataElement]
    public long ModelEntry;

    [DataElement]
    public string Name;

    [DataElement]
    public long Quality;

    [DataElement]
    public long Type;

    [DataElement]
    public long Slot;

    [DataElement]
    public long Stack;

    [DataElement]
    public string Binding;

    [DataElement]
    public long Unique;
}

