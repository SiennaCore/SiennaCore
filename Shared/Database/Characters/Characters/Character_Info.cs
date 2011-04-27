using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Characters_Positions", PreCache = true)]
[Serializable]
public class Character_Position : DataObject
{
    [PrimaryKey()]
    public int Id;

    [DataElement()]
    public long MapId;

    [DataElement()]
    public float X;

    [DataElement()]
    public float Y;

    [DataElement()]
    public float Z;

    [DataElement()]
    public float OX;

    [DataElement()]
    public float OY;

    [DataElement()]
    public float OZ;
}
