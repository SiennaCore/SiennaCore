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
                Name = "Imperium";
                RiftId = 2564;
                return; ;
            case 2:
                Name = "Scarhide";
                RiftId = 2562;
                return;
            case 3:
                Name = "heatherfield";
                RiftId = 2583;
                return;
            case 4:
                Name = "Tahkaat";
                RiftId = 2582;
                return;
            case 5:
                Name = "Cinderon";
                RiftId = 2584;
                return;
            case 6:
                Name = "maidenfalls";
                RiftId = 2572;
                return;
            case 7:
                Name = "Sparkwing";
                RiftId = 2563;
                return;
            case 8:
                Name = "Sagespire";
                RiftId = 2504;
                return; ;
            case 9:
                Name = "Firesand";
                RiftId = 2522;
                return;
            case 10:
                Name = "Feenring";
                RiftId = 2542;
                return;
            case 11:
                Name = "Refuge";
                RiftId = 2591;
                return;
            case 12:
                Name = "Mordant";
                RiftId = 2571;
                return;
            case 13:
                Name = "Overlook";
                RiftId = 2561;
                return;
            case 14:
                Name = "Icewatch";
                RiftId = 2593;
                return;
            case 15:
                Name = "Riptalon";
                RiftId = 2514;
                return;
            case 16:
                Name = "Quarrystone";
                RiftId = 2574;
                return;
            case 17:
                Name = "Bloodiron";
                RiftId = 2521;
                return;
            case 18:
                Name = "Cloudborne";
                RiftId = 2502;
                return;
            case 19:
                Name = "Shivermere";
                RiftId = 2512;
                return;
            case 20:
                Name = "Zareph";
                RiftId = 2613;
                return;
            case 21:
                Name = "Phynnious";
                RiftId = 2632;
                return;
            case 22:
                Name = "Steampike";
                RiftId = 2503;
                return;
			case 23:
                Name = "Icewatch";
                RiftId = 2511;
                return;
            case 24:
                Name = "Rhazade";
                RiftId = 2533;
                return;
            case 25:
                Name = "Argent";
                RiftId = 2513;
                return;
            case 26:
                Name = "Centius";
                RiftId = 2631;
                return;
            case 27:
                Name = "Felsspitze";
                RiftId = 2603;
                return;
            case 28:
                Name = "Blightweald";
                RiftId = 2501;
                return;
            case 29:
                Name = "Grimnir";
                RiftId = 2634;
                return;
            case 30:
                Name = "Tempête";
                RiftId = 2554;
                return;
            case 31:
                Name = "Trübkopf";
                RiftId = 2532;
                return;
            case 32:
                Name = "Spross-Passage";
                RiftId = 2534;
                return;
            case 33:
                Name = "Akala";
                RiftId = 2544;
                return;
            case 34:
                Name = "Cestus";
                RiftId = 2553;
                return;
            case 35:
                Name = "Rubicon";
                RiftId = 2552;
                return;
            case 36:
                Name = "Granitstaub";
                RiftId = 2611;
                return;
            case 37:
                Name = "Brutwacht";
                RiftId = 2531;
                return;
            case 38:
                Name = "Brutmutter";
                RiftId = 2601;
                return;
            case 39:
                Name = "Immerwacht";
                RiftId = 2541;
                return;
            case 40:
                Name = "Brisesol";
                RiftId = 2551;
                return;
            case 41:
                Name = "Whitefall";
                RiftId = 2523;
                return;

            default:
                Name = "SiennaCore custom";
                RiftId = RealmId;
                break;
        };
    }
}