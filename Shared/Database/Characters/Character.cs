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
            string Str = "";
            foreach (byte b in bBody)
                Str += b.ToString("X2") + " ";

            return Str;
        }
        set
        {
            string[] Str = value.Split(' ');
            List<byte> Bytes = new List<byte>();
            foreach (string sb in Str)
                if (sb.Length > 0)
                    Bytes.Add(byte.Parse(sb,System.Globalization.NumberStyles.HexNumber));

            bBody = Bytes.ToArray();
        }
    }

    public byte[] bBody;

}