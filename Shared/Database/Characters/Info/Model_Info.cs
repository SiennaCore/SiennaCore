using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Model_Info", PreCache = true)]
[Serializable]
public class Model_Info : DataObject
{
    [DataElement]
    public long ModelID;

    [DataElement]
    public long Race;

    [DataElement]
    public long Sex;

    [DataElement]
    public long Field_4;

    [DataElement]
    public long Field_5;

    [DataElement]
    public long Field_6;

    [DataElement]
    public long Field_7;

    [DataElement]
    public long CacheID;
}
