using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "cachedata", PreCache = true)]
[Serializable]
public class CacheEntry : DataObject
{
    [DataElement]
    public long Opcode;

    [DataElement]
    public long Type;

    [DataElement]
    public long CacheId;

    [DataElement]
    public int OnRead;

    [DataElement]
    public int SendAllways;
}