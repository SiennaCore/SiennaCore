using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    [ISerializableAttribute((long)Opcodes.WorldChatMessage)]
    public class WorldChatMessage : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ChannelName;

        [ArrayBit(2)]
        public string PlayerName;

        [ArrayBit(3)]
        public string Message;

        [Unsigned7Bit(7)]
        public long Unk1;

        [Unsigned7Bit(9)]
        public long Unk2;

        public override void OnRead(RiftClient From)
        {
            if (Message.StartsWith(".teleport"))
            {
                string CmdArgs = Message.Substring(10);
                string[] pos = CmdArgs.Split(new char[] { ' ' });

                WorldServerPositionUpdate WPos = new WorldServerPositionUpdate();

                WPos.GUID = 123456;

                WPos.Position = new List<float>();
                WPos.Position.Add(float.Parse(pos[0]));
                WPos.Position.Add(float.Parse(pos[1]));
                WPos.Position.Add(float.Parse(pos[2]));

                WPos.Orientation = new List<float>();
                WPos.Orientation.Add(0.0f);
                WPos.Orientation.Add(0.0f);
                WPos.Orientation.Add(0.0f);

                /*if (long.Parse(pos[3]) != 676)
                {
                    WorldTeleport WorldPort = new WorldTeleport();
                    WorldPort.MapId = long.Parse(pos[3]);
                    WorldPort.DestinationData = new List<ISerializablePacket>();

                    ISerializablePacket Packet = new ISerializablePacket();
                    Packet.Opcode = 0x11CE;
                    Packet.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)100);
                    Packet.AddField(1, EPacketFieldType.ByteArray, "tm_Sanctum_SanctumOfTheVigil");
                    Packet.AddField(2, EPacketFieldType.Raw8Bytes, (UInt64)9223372039941789842);

                    WorldPort.DestinationData.Add(Packet);

                    From.SendSerialized(Packet);

                    WorldZoneInfo ZoneInfo = CacheMgr.Instance.GetZoneInfoCache("Capital1");
                    From.SendSerialized(ZoneInfo);

                    WorldStartingPosition StartPos = new WorldStartingPosition();
                    StartPos.MapName = "world";

                    From.SendSerialized(StartPos);

                    WorldPositionExtra StartPos2 = new WorldPositionExtra();
                    StartPos2.MapName = "world";
                    StartPos2.MapId = 2;
                    StartPos2.Position = WPos.Position;

                    List<float> QuaternionBase = new List<float>(WPos.Orientation.ToArray());
                    QuaternionBase.Add(0.0f);

                    StartPos2.Field4 = QuaternionBase;
                    StartPos2.Field8 = WPos.Position;

                    From.SendSerialized(StartPos2);

                }
                else*/
                    From.SendSerialized(WPos);
            }
            else
            {
                PlayerName = "Magetest";
                Unk1 = 2551;
                Unk2 = 2551;
                From.SendSerialized(this);
            }
        }
    }
}
