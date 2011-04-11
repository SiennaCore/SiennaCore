using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "CacheData", PreCache = true)]
[Serializable]
public class CacheData : DataObject
{
    [DataElement()]
    public uint CacheId;

    [DataElement()]
    public long CacheType;

    [DataElement()]
    public string Name;

    [DataElement()]
    public string Description;
}
