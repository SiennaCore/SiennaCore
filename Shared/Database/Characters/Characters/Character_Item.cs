using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Characters_Items", PreCache = true)]
[Serializable]
public class Character_Item : DataObject
{
    [DataElement]
    public long GUID;

    [DataElement]
    public long ItemID;

    [DataElement]
    public long Bag;

    [DataElement]
    public long Equiped;
}
