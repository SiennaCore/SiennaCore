using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName="Accounts",TableName="Accounts",PreCache=true)]
[Serializable]
public class Account : DataObject
{
    [PrimaryKey(AutoIncrement=true)]
    public long Id;

    [DataElement(Unique=true)]
    public string Username;

    [DataElement]
    public string Sha_Password;

    [DataElement]
    public string SessionKey;

    [DataElement]
    public long SessionTicket;

    [DataElement]
    public string Email;

    [DataElement]
    public int GmLevel;

    [DataElement]
    public long PendingCharacter;

    [Relation(AutoDelete = true, AutoLoad = true, LocalField = "Id", RemoteField = "Id")]
    public Account_banned[] Bans;
}