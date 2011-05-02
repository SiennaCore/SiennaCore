using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Characters_StartItems", PreCache = true)]
[Serializable]
public class Character_StartItems : DataObject
{
    [DataElement]
    public long Race;

    [DataElement]
    public long Sex;

    [DataElement]
    public long Class;

    [DataElement]
    public long ItemID;
}
