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

    [DataElement(Varchar = 9)]
    public string Name;

    [DataElement]
    public byte Race;

    [DataElement]
    public byte Class;

    [DataElement]
    public byte Sex;

    [DataElement]
    public string Body
    {
        get
        {
            return Encoding.ASCII.GetString(bBody);
        }
        set
        {
            bBody = Encoding.ASCII.GetBytes(value);
        }
    }

    public byte[] bBody;

}