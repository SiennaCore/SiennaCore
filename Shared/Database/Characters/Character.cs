using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Characters", PreCache = true)]
[Serializable]
public class Character : DataObject
{
    [PrimaryKey(AutoIncrement = true)]
    public int Id;

    [DataElement]
    public long AccountId;

    [DataElement]
    public byte RealmId;

    [DataElement(Varchar = 19)]
    public string Name;

    [DataElement()]
    public long Race;

    [DataElement()]
    public long Classe;

    [DataElement()]
    public long Level;

    [DataElement()]
    public string Data;
}