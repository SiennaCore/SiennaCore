using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterCreationCacheRequest)]
    public class LobbyCharacterCreationCacheRequest : ISerializablePacket
    {
        public override void OnRead(RiftClient From)
        {
            Log.Success("CreationCache", "Request");

            SendCache(From,7310, 1669417405);
            SendCache(From,623, 768949022);
            SendCache(From,7310, 675891729);
            SendCache(From,623, 1577313627);
            SendCache(From,7310, 1975118634);
            SendCache(From,623, 867589601);
            SendCache(From,7310, 169931229);
            SendCache(From,623, 699634871);
            SendCache(From,7310, 1880107978);
            SendCache(From,623, 1188780851);
            SendCache(From,7310, 1667530612);
            SendCache(From,623, 1933503643);
            SendCache(From,7310, 528356673);
            SendCache(From,623, 1325542751);
            SendCache(From,7310, 874794657);
            SendCache(From,623, 2093007487);
            SendCache(From,7310, 2086263037);
            SendCache(From,623, 644844382);
            SendCache(From,7310, 844935536);
            SendCache(From,623, 390203786);
            SendCache(From,7310, 1557697584);
            SendCache(From,623, 1443859229);
            SendCache(From,7310, 1526380191);
            SendCache(From,623, 221597436);
            SendCache(From,7310, 1129832166);
            SendCache(From,623, 1117191825);
            SendCache(From,7310, 1864652718);
            SendCache(From,623, 586599475);
            SendCache(From,7310, 1813805611);
            SendCache(From,623, 2028933878);
            SendCache(From,7310, 1445588088);
            SendCache(From,623, 1979672106);
            SendCache(From,7310, 2102616674);
            SendCache(From,623, 632628948);
            SendCache(From,7310, 252263437);
            SendCache(From,623, 493821701);
            SendCache(From,7310, 1254989851);
            SendCache(From,623, 1648876373);
            SendCache(From,7310, 1171609220);
            SendCache(From,623, 1963778821);
            SendCache(From,7310, 506516265);
            SendCache(From,623, 456805300);
            SendCache(From,7310, 405979626);
            SendCache(From,623, 1817907681);
            SendCache(From,7310, 1060014307);
            SendCache(From,623, 2116922465);
            SendCache(From,7310, 1776647175);
            SendCache(From,623, 890018716);
            SendCache(From,623, 342526619);
            SendCache(From,623, 1752069204);
            SendCache(From,623, 1206373691);
            SendCache(From,623, 620427584);
            SendCache(From,623, 1953390095);
            SendCache(From,623, 2091212398);
            SendCache(From,623, 807819347);
            SendCache(From,623, 517233896);
            SendCache(From,623, 1518510797);
            SendCache(From,7310, 9797740);
            SendCache(From,623, 1201656245);
            SendCache(From,623, 1964694620);
            SendCache(From,7310, 80037963);
            SendCache(From,7310, 1994909705);
            SendCache(From,7310, 276418120);
            SendCache(From,623, 1267083491);
            SendCache(From,623, 454009772);
            SendCache(From,7310, 2129609409);
            SendCache(From,623, 2051750519);
            SendCache(From,623, 1018745354);
            SendCache(From,7310, 38);
            SendCache(From,7310, 1383643167);
            SendCache(From,623, 1698210025);
            SendCache(From,7310, 37);
            SendCache(From,7310, 122289716);
            SendCache(From,623, 782363744);
            SendCache(From,623, 302);
            SendCache(From,623, 522019961);
            SendCache(From,7310, 1106486525);
            SendCache(From,7310, 460838536);
            SendCache(From,7310, 789458208);
            SendCache(From,623, 1576431708);
            SendCache(From,623, 280617409);
            SendCache(From,623, 37229814);
            SendCache(From,7310, 249606367);
            SendCache(From,7310, 648397088);
            SendCache(From,623, 1840006736);
            SendCache(From,623, 1246019929);
            SendCache(From,7310, 990666439);
            SendCache(From,7310, 946275249);
            SendCache(From,7310, 535223294);
            SendCache(From,7310, 1469269995);
            SendCache(From,7310, 76600761);
            SendCache(From,7310, 1824770691);
            SendCache(From,7310, 604022000);
            SendCache(From,7310, 1895424791);
            SendCache(From,623, 414973049);
            SendCache(From,623, 796158398);
            SendCache(From,623, 1329031367);
            SendCache(From,623, 1319819530);
            SendCache(From,623, 1984890248);
            SendCache(From,623, 1918905243);
            SendCache(From,623, 726093410);
            SendCache(From,623, 1845828446);
            SendCache(From,7310, 895630649);
            SendCache(From,7310, 1075374648);
            SendCache(From,7310, 426961537);
            SendCache(From,7310, 752559972);
            SendCache(From,7310, 787185854);
            SendCache(From,7310, 1645606026);
            SendCache(From,7310, 602607326);
            SendCache(From,623, 4660758);
            SendCache(From,623, 2065208883);
            SendCache(From,623, 782260305);
            SendCache(From,623, 729123112);
            SendCache(From,623, 1547313675);
            SendCache(From,7310, 1807028507);
            SendCache(From,623, 672742918);
            SendCache(From,623, 942274209);
            SendCache(From,623, 2014860225);
            SendCache(From,623, 662740452);
            SendCache(From,623, 644238304);
            SendCache(From,623, 1094278633);
            SendCache(From,623, 1923367289);
            SendCache(From,623, 1480811393);
            SendCache(From,623, 2059768682);
            SendCache(From,623, 2071263030);
            SendCache(From,623, 1336345388);
            SendCache(From,623, 999573357);
            SendCache(From,623, 1795502664);
            SendCache(From,623, 741209751);
            SendCache(From,623, 105489935);
            SendCache(From,7310, 1196150716);
            SendCache(From,623, 1813788537);
            SendCache(From,7310, 1884106754);
            SendCache(From,623, 545806888);
            SendCache(From,7310, 1957963062);
            SendCache(From,623, 1571269277);
            SendCache(From,7310, 1794752650);
            SendCache(From,623, 1140003812);
            SendCache(From,7310, 621800394);
            SendCache(From,623, 919356075);
            SendCache(From,7310, 1088944139);
            SendCache(From,623, 824725952);

            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = (long)Opcodes.LobbyCharacterCreationCacheResponse;
            From.SendSerialized(Packet);
        }

        public void SendCache(RiftClient From,long CacheType, uint ID)
        {
            byte[] Packet = CharacterMgr.Instance.GetCache(CacheType, ID);
            if (Packet == null)
                return;

            From.SendTCPWithSize(Packet);
        }
    }
}
