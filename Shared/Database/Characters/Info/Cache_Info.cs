using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Cache_Info", PreCache = true)]
[Serializable]
public class Cache_Info : DataObject
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