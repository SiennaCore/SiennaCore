/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using FrameWork;

[DataTable(DatabaseName = "Accounts", TableName = "Realms", PreCache = true)]
[Serializable]
public class Realm : DataObject
{
    [PrimaryKey]
    public byte RealmId;

    [DataElement(AllowDbNull = false)]
    public byte PVP;

    [DataElement(AllowDbNull = false)]
    public byte RP;

    // 1 = English
    // 3 = German
    // 5 = French
    [DataElement(AllowDbNull = false)]
    public byte Language;

    [XmlIgnore()]
    [DataElement(AllowDbNull = false)]
    public byte Online
    {
        get
        {
            return (byte)(RpcInfo != null ? 1 : 0);
        }
        set
        {
            
        }
    }

    [DataElement(AllowDbNull = false)]
    public byte Recommended;

    [DataElement(AllowDbNull = false)]
    public long ClientVersion;

    #region Name

    [XmlIgnore()]
    public long RiftId;

    [XmlIgnore()]
    public string Name;

    public void GenerateName()
    {
        Dirty = true;

        switch (RealmId)
        {
            case 1:
                Name = "Imperium";
                RiftId = 2564;
                break;

            case 2:
                Name = "Scarhide";
                RiftId = 2562;
                break;

            case 3:
                Name = "heatherfield";
                RiftId = 2583;
                break;

            case 4:
                Name = "Tahkaat";
                RiftId = 2582;
                break;

            case 5:
                Name = "Cinderon";
                RiftId = 2584;
                break;

            case 6:
                Name = "maidenfalls";
                RiftId = 2572;
                break;

            case 7:
                Name = "Sparkwing";
                RiftId = 2563;
                break;

            case 8:
                Name = "Sagespire";
                RiftId = 2504;
                break;

            case 9:
                Name = "Firesand";
                RiftId = 2522;
                break;

            case 10:
                Name = "Feenring";
                RiftId = 2542;
                break;

            case 11:
                Name = "Refuge";
                RiftId = 2591;
                break;

            case 12:
                Name = "Mordant";
                RiftId = 2571;
                break;

            case 13:
                Name = "Overlook";
                RiftId = 2561;
                break;

            case 14:
                Name = "Icewatch";
                RiftId = 2593;
                break;

            case 15:
                Name = "Riptalon";
                RiftId = 2514;
                break;

            case 16:
                Name = "Quarrystone";
                RiftId = 2574;
                break;

            case 17:
                Name = "Bloodiron";
                RiftId = 2521;
                break;

            case 18:
                Name = "Cloudborne";
                RiftId = 2502;
                break;

            case 19:
                Name = "Shivermere";
                RiftId = 2512;
                break;

            case 20:
                Name = "Zareph";
                RiftId = 2613;
                break;

            case 21:
                Name = "Phynnious";
                RiftId = 2632;
                break;

            case 22:
                Name = "Steampike";
                RiftId = 2503;
                break;

            case 23:
                Name = "Icewatch";
                RiftId = 2511;
                break;

            case 24:
                Name = "Rhazade";
                RiftId = 2533;
                break;

            case 25:
                Name = "Argent";
                RiftId = 2513;
                break;

            case 26:
                Name = "Centius";
                RiftId = 2631;
                break;

            case 27:
                Name = "Felsspitze";
                RiftId = 2603;
                break;

            case 28:
                Name = "Blightweald";
                RiftId = 2501;
                break;

            case 29:
                Name = "Grimnir";
                RiftId = 2634;
                break;

            case 30:
                Name = "Tempête";
                RiftId = 2554;
                break;

            case 31:
                Name = "Trübkopf";
                RiftId = 2532;
                break;

            case 32:
                Name = "Spross-Passage";
                RiftId = 2534;
                break;

            case 33:
                Name = "Akala";
                RiftId = 2544;
                break;

            case 34:
                Name = "Cestus";
                RiftId = 2553;
                break;

            case 35:
                Name = "Rubicon";
                RiftId = 2552;
                break;

            case 36:
                Name = "Granitstaub";
                RiftId = 2611;
                break;

            case 37:
                Name = "Brutwacht";
                RiftId = 2531;
                break;

            case 38:
                Name = "Brutmutter";
                RiftId = 2601;
                break;

            case 39:
                Name = "Immerwacht";
                RiftId = 2541;
                break;

            case 40:
                Name = "Brisesol";
                RiftId = 2551;
                break;

            case 41:
                Name = "Whitefall";
                RiftId = 2523;
                break;


            default:
                Name = "SiennaCore custom";
                RiftId = RealmId;
                break;
        };
    }

    #endregion

    #region Remoting

    [XmlIgnore()]
    public RpcClientInfo RpcInfo;

    public T GetObject<T>() where T : RpcObject
    {
        return RpcServer.GetObject<T>(RpcInfo);
    }

    #endregion
}

