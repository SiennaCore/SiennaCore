using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Zone_Info", PreCache = true)]
[Serializable]
public class Zone_Info : DataObject
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

