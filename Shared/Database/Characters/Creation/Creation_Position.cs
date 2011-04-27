using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Creation_Positions", PreCache = true)]
[Serializable]
public class Creation_Position : DataObject
{
    [PrimaryKey()]
    public long Faction;

    [DataElement()]
    public int MapId;

    [DataElement()]
    public float X;

    [DataElement()]
    public float Y;

    [DataElement()]
    public float Z;
}