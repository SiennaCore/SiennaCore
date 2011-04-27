using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldTeleport)]
    public class WorldTeleport : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long MapId;

        [ListBit(1)]
        public List<ISerializablePacket> TeleportInformations;

        public static WorldTeleport FromPorticulum(long MapId, long NPCId, long Cost, string ZoneData)
        {
            WorldTeleport Wteleport = new WorldTeleport();

            WorldPorticulumConfirm WorldPortConf = new WorldPorticulumConfirm();
            WorldPortConf.Cost = Cost;
            WorldPortConf.NPCId = NPCId;
            WorldPortConf.ZoneData = ZoneData; 
            Wteleport.MapId = MapId;
            Wteleport.TeleportInformations = new List<ISerializablePacket>();
            Wteleport.TeleportInformations.Add(WorldPortConf);

            return Wteleport;
        }

        public override void OnRead(RiftClient From)
        {
            
        }
    }
}
