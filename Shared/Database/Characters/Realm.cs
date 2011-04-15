using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;
using Shared.NetWork;
using Shared.Database;

[Serializable]
[DataTable(DatabaseName = "Characters", TableName = "Realms", PreCache = true)]
[ISerializableAttribute((long)Opcodes.LobbyWorldEntry)]
public class Realm : DataObject
{
    [PrimaryKey]
    public byte RealmId;

    public string Name;

    [DataElement(Varchar = 255)]
    public string Address;

    [DataElement(AllowDbNull = false)]
    public byte Language;
    // 1 = English
    // 3 = German
    // 5 = French

    [DataElement(AllowDbNull = false)]
    public long ClientVersion;

    [DataElement(AllowDbNull=false)]
    public byte PVP;

    [DataElement(AllowDbNull = false)]
    public byte RP;

    [DataElement(AllowDbNull=false)]
    public byte Online;

    [DataElement(AllowDbNull = false)]
    public byte Recommended;

    // Remoting
    public int RpcId;

    public long RiftId;

    public void GenerateName()
    {
        Dirty = true;

        switch (RealmId)
        {
            case 1:
                Name = "Mordant";
                RiftId = 2571;
                return; ;
            case 2:
                Name = "Refuge";
                RiftId = 2591;
                return;
            case 3:
                Name = "Quarrystone";
                RiftId = 2574;
                return;
            case 4:
                Name = "Heaterfield";
                RiftId = 2583;
                return;
            case 5:
                Name = "Maidenfalls";
                RiftId = 2572;
                return;

            case 6:
                Name = "Brisesol";
                RiftId = 2514;
                return;
            case 7:
                Name = "Icewatch";
                RiftId = 2593;
                return;

            case 8:
                Name = "Rubicon";
                RiftId = 2582;
                return; ;
            case 9:
                Name = "Brutwatch";
                RiftId = 55057;
                return;
            case 10:
                Name = "Trûbkopf";
                RiftId = 55313;
                return;
            case 11:
                Name = "Argent";
                RiftId = 57105;
                return;
            case 12:
                Name = "Firesand";
                RiftId = 57361;
                return;
            case 13:
                Name = "Sagespire";
                RiftId = 57617;
                return;
            case 14:
                Name = "Illumintation";
                RiftId = 57873;
                return;
            case 15:
                Name = "Tempête";
                RiftId = 59665;
                return;
            case 16:
                Name = "Riptalon";
                RiftId = 59921;
                return;
            case 17:
                Name = "Akala";
                RiftId = 60177;
                return;
            case 18:
                Name = "Feenring";
                RiftId = 60433;
                return;
            case 19:
                Name = "Poudre-granite";
                RiftId = 60178;
                return;
            case 20:
                Name = "Tempêtueux";
                RiftId = 45330;
                return;
            case 21:
                Name = "Sprosspassage";
                RiftId = 47634;
                return;
            case 22:
                Name = "Kristaltiefiend";
                RiftId = 47890;
                return;

            default:
                Name = "Custom";
                RiftId = RealmId;
                break;
        };
    }
}