using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "ItemCache_Info", PreCache = true)]
[Serializable]
public class ItemCache_Info : DataObject
{
    [DataElement]
    public long ItemID;

    [DataElement]
    public long CacheID;
}
