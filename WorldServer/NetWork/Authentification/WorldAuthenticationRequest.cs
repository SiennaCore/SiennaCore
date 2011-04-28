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

            WorldCacheUpdated Updated = new WorldCacheUpdated();
            Updated.GUID = CharacterId;
            From.SendSerialized(Updated);

            /**** One of them seem to delete object ***/

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

            /********************************************/
            
            WorldZoneInfo ZoneInfo = CacheMgr.Instance.GetZoneInfoCache("Mathosia1");
            From.SendSerialized(ZoneInfo);

            WorldStartingPosition StartPosition = new WorldStartingPosition();
            StartPosition.MapName = "guardian_map";
            From.SendSerialized(StartPosition);

            WorldPositionExtra ExtraPosition = new WorldPositionExtra();
            ExtraPosition.MapName = "guardian_map";

            ISerializablePacket Extra = new ISerializablePacket();
            Extra.Opcode = (long)Opcodes.WorldStartingPositionExtra;
            Extra.AddField(0, EPacketFieldType.Packet, ExtraPosition);
            From.SendSerialized(Extra);
        }
    }
}
