using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "textentry", PreCache = true)]
[Serializable]
public class TextEntry : DataObject
{
    [PrimaryKey(AutoIncrement = true)]
    public long Id;

    [DataElement]
    public string Loc_1;

    [DataElement]
    public string Loc_2;

    [DataElement]
    public string Loc_3;
}
