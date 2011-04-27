using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "zoneentry", PreCache = true)]
[Serializable]
public class ZoneEntry : DataObject
{
    [DataElement]
    public long MapId;

    [DataElement]
    public string Name;

    [DataElement]
    public long DisplayName;

    [DataElement]
    public long Desc;
}

