using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Shared;
using Shared.Database;
using Shared.NetWork;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldAuthenticationResponse)]
    public class WorldAuthenticationResponse : ISerializablePacket
    {
    }

    [ISerializableAttribute((long)Opcodes.WorldAuthenticationRequest)]
    public class WorldAuthenticationRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Email;

        [Unsigned7Bit(1)]
        public long CharacterId=123456;

        public override void OnRead(RiftClient From)
        {
            Log.Success("Authentification", "Email = " + Email);

            WorldAuthenticationResponse Rp = new WorldAuthenticationResponse();
            Rp.AddField(0, EPacketFieldType.True, (bool)true);
            From.SendSerialized(Rp);

            From.SendCache(7310, 1954446864);
            From.SendCache(623, 137130154);
            From.SendCache(623, 1813202267);
            From.SendCache(623, 2144324383);
            From.SendCache(623, 973462583);
            From.SendCache(7310, 1768661154);
            From.SendCache(623, 1287831893);
            From.SendCache(7310, 2007607340);
            From.SendCache(623, 935832306);
            From.SendCache(7310, 1259168216);
            From.SendCache(623, 1462063433);
            From.SendCache(7310, 1667530612);
            From.SendCache(623, 1933503643);
            From.SendCache(7310, 528356673);
            From.SendCache(623, 1325542751);
            From.SendCache(7310, 1669417405);
            From.SendCache(623, 768949022);
            From.SendCache(7310, 874794657);
            From.SendCache(623, 2093007487);
            From.SendCache(7310, 2086263037);
            From.SendCache(623, 644844382);
            From.SendCache(7310, 844935536);
            From.SendCache(623, 390203786);
            From.SendCache(7310, 1776647175);
            From.SendCache(623, 2091212398);
            From.SendCache(7310, 722839613);
            From.SendCache(623, 722839613);

            // Unk
            {
                WorldCacheUpdated Updated = new WorldCacheUpdated();
                Updated.GUID = CharacterId;
                From.SendSerialized(Updated);

               /* ISerializablePacket Packet1 = new ISerializablePacket();
                Packet1.Opcode = 0x03EB;
                Packet1.AddField(0, EPacketFieldType.Raw8Bytes, new byte[8] { 0xBC, 0x3C, 0x09, 0x06, 0x00, 0x80, 0x23, 0x05 });
                Packet1.AddField(1, EPacketFieldType.True, (bool)true);
                From.SendSerialized(Packet1);*/

                ISerializablePacket Packet1 = new ISerializablePacket();
                Packet1.Opcode = 0x03F6;
                Packet1.AddField(0, EPacketFieldType.Raw4Bytes, new byte[4] { 0x00, 0x0C, 0xE8, 0x40 });
                Packet1.AddField(1, EPacketFieldType.ByteArray, new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                                                                             00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                                                                             00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
                Packet1.AddField(2, EPacketFieldType.Raw8Bytes, new byte[8] { 0xCB, 0x34, 0x3D, 0x94, 0x23, 0x04, 0xCC, 0x01 });
                From.SendSerialized(Packet1);

                ISerializablePacket Packet2 = new ISerializablePacket();
                Packet2.Opcode = 0x02E9;
                Packet2.AddField(0, EPacketFieldType.List, new List<long>() { 3605869292 });
                From.SendSerialized(Packet2);

                ISerializablePacket Packet3 = new ISerializablePacket();
                Packet3.Opcode = 0x2D7F;
                From.SendSerialized(Packet3);

                /*ISerializablePacket Packet4 = new ISerializablePacket();
                Packet4.Opcode = 0x1973;
                From.SendSerialized(Packet4);*/
            }

            // Map Info
            {
                WorldMapInfo MapInfo1 = new WorldMapInfo();
                MapInfo1.MapId = 1647389394;
                MapInfo1.Text = "Long ago, the heartland of the Mathosian empire was torn by civil war.";

                WorldMapInfo MapInfo9 = new WorldMapInfo();
                MapInfo9.MapId = 290412351;
                MapInfo9.Text = "Mathosia";

                WorldZoneInfo ZoneInfo = new WorldZoneInfo();
                ZoneInfo.MapFileName = "Mathosia1";
                ZoneInfo.AddField(1, EPacketFieldType.Packet, MapInfo1);
                ZoneInfo.AddField(9, EPacketFieldType.Packet, MapInfo9);
                From.SendSerialized(ZoneInfo);
            }


            WorldStartingPosition StartPosition = new WorldStartingPosition();
            StartPosition.MapName = "guardian_map";
            StartPosition.Position = new List<uint>() { 1149965263, 1147537926, 1152778324 };
            From.SendSerialized(StartPosition);

            WorldPositionExtra ExtraPosition = new WorldPositionExtra();
            ExtraPosition.MapName = "guardian_map";

            ISerializablePacket Extra = new ISerializablePacket();
            Extra.Opcode = (long)Opcodes.WorldStartingPositionExtra;
            Extra.AddField(0, EPacketFieldType.Packet, ExtraPosition);
            From.SendSerialized(Extra);

            // Cache
            {
                /*From.SendCache(711, 1942532282);
                From.SendCache(7345, 1410791552);
                From.SendCache(623, 1410791552);
                From.SendCache(7345, 213376840);
                From.SendCache(623, 213376840);
                From.SendCache(7345, 1289235358);
                From.SendCache(623, 1289235358);
                From.SendCache(7345, 1887579901);
                From.SendCache(623, 1887579901);
                From.SendCache(7345, 2018212493);
                From.SendCache(623, 2018212493);
                From.SendCache(7345, 2098634216);
                From.SendCache(623, 2098634216);
                From.SendCache(7345, 2098634217);
                From.SendCache(623, 2098634217);
                From.SendCache(7345, 1018970202);
                From.SendCache(623, 1018970202);
                From.SendCache(7345, 2098634218);
                From.SendCache(623, 2098634218);
                From.SendCache(7345, 2098634215);
                From.SendCache(623, 2098634215);
                From.SendCache(7345, 1355836664);
                From.SendCache(623, 1355836664);
                From.SendCache(7345, 2053795146);
                From.SendCache(623, 2053795146);
                From.SendCache(7345, 2098634219);
                From.SendCache(623, 2098634219);
                From.SendCache(7345, 966762906);
                From.SendCache(623, 966762906);
                From.SendCache(7345, 1148849124);
                From.SendCache(623, 1148849124);
                From.SendCache(7345, 1438051495);
                From.SendCache(623, 1438051495);
                From.SendCache(7345, 1228643830);
                From.SendCache(623, 1228643830);
                From.SendCache(7345, 1416711275);
                From.SendCache(623, 1416711275);
                From.SendCache(7345, 1416711276);
                From.SendCache(623, 1416711276);
                From.SendCache(7345, 1355836665);
                From.SendCache(623, 1355836665);
                From.SendCache(7345, 1743212613);
                From.SendCache(623, 1743212613);
                From.SendCache(711, 2027684872);
                From.SendCache(711, 637022422);
                From.SendCache(741, 1048068101);
                From.SendCache(7345, 2044445417);
                From.SendCache(623, 2044445417);
                From.SendCache(711, 1200016184);
                From.SendCache(711, 1173218035);
                From.SendCache(7345, 1599307652);
                From.SendCache(623, 1599307652);
                From.SendCache(711, 1123289813);
                From.SendCache(711, 1760647327);
                From.SendCache(7345, 1864406151);
                From.SendCache(623, 1864406151);
                From.SendCache(711, 580579355);
                From.SendCache(711, 845194610);
                From.SendCache(7345, 2110568755);
                From.SendCache(623, 2110568755);
                From.SendCache(711, 1572264459);
                From.SendCache(711, 439761185);
                From.SendCache(7345, 232089578);
                From.SendCache(623, 232089578);
                From.SendCache(711, 1291828278);
                From.SendCache(711, 1891591571);
                From.SendCache(7345, 333586647);
                From.SendCache(623, 333586647);
                From.SendCache(711, 1501187355);
                From.SendCache(711, 1477802084);
                From.SendCache(741, 894194477);
                From.SendCache(7345, 1153815452);
                From.SendCache(623, 1153815452);
                From.SendCache(711, 1821585178);
                From.SendCache(711, 1282127124);
                From.SendCache(711, 2013356786);
                From.SendCache(7345, 845728233);
                From.SendCache(623, 845728233);
                From.SendCache(7345, 110642793);
                From.SendCache(623, 110642793);
                From.SendCache(711, 1934582362);
                From.SendCache(711, 1167372615);
                From.SendCache(741, 1686565888);
                From.SendCache(7345, 1154171760);
                From.SendCache(623, 1154171760);
                From.SendCache(711, 1418226102);
                From.SendCache(741, 8774349);
                From.SendCache(711, 819095324);
                From.SendCache(7345, 1465435011);
                From.SendCache(623, 1465435011);
                From.SendCache(711, 1660627555);
                From.SendCache(711, 1263632211);
                From.SendCache(741, 499863559);
                From.SendCache(7345, 1450622755);
                From.SendCache(623, 1450622755);
                From.SendCache(711, 1882708243);
                From.SendCache(741, 1318266367);
                From.SendCache(711, 2038895381);
                From.SendCache(7345, 589448162);
                From.SendCache(623, 589448162);
                From.SendCache(711, 1088123401);
                From.SendCache(711, 393659851);
                From.SendCache(741, 1894353703);
                From.SendCache(7345, 1549365785);
                From.SendCache(623, 1549365785);
                From.SendCache(711, 921533429);
                From.SendCache(711, 595494881);
                From.SendCache(741, 1667488803);
                From.SendCache(7345, 25470989);
                From.SendCache(623, 25470989);
                From.SendCache(711, 1623549711);
                From.SendCache(711, 1454521163);
                From.SendCache(741, 670226621);
                From.SendCache(7345, 2016668894);
                From.SendCache(623, 2016668894);
                From.SendCache(711, 2122307999);
                From.SendCache(711, 983640933);
                From.SendCache(741, 82695784);
                From.SendCache(7345, 1263297985);
                From.SendCache(623, 1263297985);
                From.SendCache(711, 250255453);
                From.SendCache(741, 1357551079);
                From.SendCache(711, 934528781);
                From.SendCache(7345, 1553212626);
                From.SendCache(623, 1553212626);
                From.SendCache(7345, 1749491619);
                From.SendCache(623, 1749491619);
                From.SendCache(7345, 885742339);
                From.SendCache(623, 885742339);
                From.SendCache(7345, 1518695738);
                From.SendCache(623, 1518695738);
                From.SendCache(7345, 1957615805);
                From.SendCache(623, 1957615805);
                From.SendCache(7345, 1304067054);
                From.SendCache(623, 1304067054);
                From.SendCache(749, 281905761);
                From.SendCache(623, 1535107391);
                From.SendCache(882, 58217189);
                From.SendCache(7310, 1794752650);
                From.SendCache(623, 1140003812);
                From.SendCache(7310, 933051248);
                From.SendCache(623, 1142719576);
                From.SendCache(7310, 1957963062);
                From.SendCache(623, 1571269277);
                From.SendCache(7310, 1088944139);
                From.SendCache(623, 824725952);
                From.SendCache(882, 132020594);
                From.SendCache(7310, 648397088);
                From.SendCache(7310, 249606367);
                From.SendCache(7310, 460838536);
                From.SendCache(7310, 1106486525);
                From.SendCache(7310, 789458208);
                From.SendCache(623, 1576431708);
                From.SendCache(623, 37229814);
                From.SendCache(623, 280617409);
                From.SendCache(623, 1246019929);
                From.SendCache(623, 1840006736);
                From.SendCache(7310, 888059302);
                From.SendCache(7310, 283104901);
                From.SendCache(7310, 804670313);
                From.SendCache(623, 2028413906);
                From.SendCache(623, 129122221);
                From.SendCache(623, 991996421);
                From.SendCache(882, 1173973622);
                From.SendCache(7310, 1895424791);
                From.SendCache(7310, 604022000);
                From.SendCache(7310, 1824770691);
                From.SendCache(7310, 76600761);
                From.SendCache(7310, 1469269995);
                From.SendCache(7310, 535223294);
                From.SendCache(7310, 946275249);
                From.SendCache(7310, 990666439);
                From.SendCache(623, 1845828446);
                From.SendCache(623, 726093410);
                From.SendCache(623, 1918905243);
                From.SendCache(623, 1984890248);
                From.SendCache(623, 1319819530);
                From.SendCache(623, 1329031367);
                From.SendCache(623, 796158398);
                From.SendCache(623, 414973049);
                From.SendCache(882, 2013120322);
                From.SendCache(7310, 895630649);
                From.SendCache(7310, 1075374648);
                From.SendCache(7310, 426961537);
                From.SendCache(7310, 752559972);
                From.SendCache(7310, 787185854);
                From.SendCache(7310, 1645606026);
                From.SendCache(7310, 602607326);
                From.SendCache(623, 4660758);
                From.SendCache(623, 2065208883);
                From.SendCache(623, 782260305);
                From.SendCache(623, 729123112);
                From.SendCache(623, 1547313675);
                From.SendCache(7310, 1807028507);
                From.SendCache(623, 672742918);
                From.SendCache(623, 942274209);
                From.SendCache(623, 2014860225);
                From.SendCache(882, 544153507);
                From.SendCache(7310, 1383643167);
                From.SendCache(7310, 122289716);
                From.SendCache(623, 782363744);
                From.SendCache(623, 522019961);
                From.SendCache(7310, 2129609409);
                From.SendCache(7310, 80037963);
                From.SendCache(7310, 1994909705);
                From.SendCache(7310, 276418120);
                From.SendCache(623, 1267083491);
                From.SendCache(623, 454009772);
                From.SendCache(623, 1018745354);
                From.SendCache(623, 2051750519);
                From.SendCache(623, 1698210025);
                From.SendCache(612, 281905761);
                From.SendCache(711, 527888188);
                From.SendCache(711, 1204101364);
                From.SendCache(711, 588069752);
                From.SendCache(711, 1739267530);
                From.SendCache(711, 726697887);
                From.SendCache(711, 1047311457);
                From.SendCache(711, 705721274);
                From.SendCache(7345, 134156705);
                From.SendCache(623, 134156705);*/

            }
        }
    }
}
