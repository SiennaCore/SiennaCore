using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Characters_Items", PreCache = true)]
[Serializable]
public class Character_Item : DataObject
{
    [DataElement()]
    public int Id;

    [DataElement()]
    public long ItemID;
}
