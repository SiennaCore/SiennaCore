using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.NetWork;
using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Realms", PreCache = true)]
[Serializable]
public class Realm : DataObject
{
    [PrimaryKey]
    public byte RealmId;

    [DataElement(Varchar = 255)]
    public string Name;

    [DataElement(Varchar = 255)]
    public string Address;

    [DataElement()]
    public byte Online;

    [DataElement(AllowDbNull = false)]
    public byte RealmType;

    [DataElement(AllowDbNull = false)]
    public byte Language;

    public int RiftId;

    public void GenerateName()
    {
        Dirty = true;

        switch (RealmId)
        {
            case 1:
                Name = "Steampike";
                RiftId = 40210;
                return; ;
            case 2:
                Name = "Whitefall";
                RiftId = 42770;
                return;
            case 3:
                Name = "Cloudborne";
                RiftId = 52241;
                return;
            case 4:
                Name = "Immerwatch";
                RiftId = 52753;
                return;
            case 5:
                Name = "Blightwild";
                RiftId = 51985;
                return;
            case 6:
                Name = "Brisesol";
                RiftId = 52497;
                return;
            case 7:
                Name = "Icewatch";
                RiftId = 54545;
                return;

            case 8:
                Name = "Rubicon";
                RiftId = 54801;
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
    public void Write(ref PacketOut Out)
    {
        Out.WriteByte(0xE5);
        Out.WriteUInt16(0x0302);
        Out.WriteUInt16((UInt16)RiftId);
        Out.WriteUInt32(0x0AB6C301);

        int RealmStatus = 0x00;

        RealmStatus = RealmStatus ^ 112; // 80 medium, 112 low, 
        RealmStatus = RealmStatus ^ 1; // 1 online 2 offline

        int RealmData = 0x00;
        RealmData = RealmData ^ RealmType; // 16 PVP, 48 PVE, 80 PVE-RP, 0 Locked
        RealmData = RealmData ^ 8;
        RealmData = RealmData ^ 1;

        Out.WriteByte((byte)RealmStatus);
        Out.WriteByte((byte)RealmData);
        Out.WriteByte(0x62);
        Out.WriteByte((byte)Language);
        Out.WriteUInt16(0x6907);
    }
}