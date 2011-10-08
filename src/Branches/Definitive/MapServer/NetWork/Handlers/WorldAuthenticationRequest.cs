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
using System.IO;

using Common;
using FrameWork;

namespace MapServer
{
    [ISerializableAttribute((long)Opcodes.WorldServerMOTD)]
    public class WorldServerMOTD : ISerializablePacket
    {
        [ArrayBit(1)]
        public string Text;
    }

    [ISerializableAttribute((long)Opcodes.WorldChannelJoinned)]
    public class WorldChannelJoinned : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ChannelName;

        [ArrayBit(1)]
        public string CharacterName;

        [Unsigned7Bit(2)]
        public long Field2 = 5;
    }

    [ISerializableAttribute((long)Opcodes.WorldTemplateUpdate)]
    public class WorldTemplateUpdate : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;

        [ArrayBit(1)]
        public byte[] Field1;
    }

    [ISerializableAttribute((long)Opcodes.WorldCanConnect)]
    public class WorldCanConnect : ISerializablePacket
    {
        static public bool test = false;
        public override void OnRead(RiftClient From)
        {
            Log.Info("WorldCanConnect", "Connecting...");
            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = 0x1E9A;
            Packet.AddField(1, EPacketFieldType.True, false);
            From.SendSerialized(Packet);
        }
    }

    [ISerializableAttribute((long)Opcodes.WorldStartingPosition)]
    public class WorldStartingPosition : ISerializablePacket
    {
        [ArrayBit(0)]
        public string MapName;

        [ListBit(1)]
        public List<uint> Position = new List<uint>() { 1149965263, 1147537926, 1152778324 }; // X,Y,Z
    }

    [ISerializableAttribute((long)Opcodes.WorldZoneInfo)]
    [Serializable]
    public class WorldZoneInfo : ISerializablePacket
    {
        [ArrayBit(0)]
        public string ZoneFileName;

        [PacketBit(1)]
        public TextInfo Description;

        [PacketBit(9)]
        public TextInfo DisplayName;


    }

    [ISerializableAttribute((long)Opcodes.WorldCacheUpdated)]
    public class WorldCacheUpdated : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;
    }

    [ISerializableAttribute((long)Opcodes.WorldAuthenticationResponse)]
    public class WorldAuthenticationResponse : ISerializablePacket
    {
    }

    [ISerializableAttribute((long)Opcodes.WorldAuthenticationRequest)]
    public class WorldAuthenticationRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Email;

        [Raw8Bit(1)]
        public long SessionTicket;

        public override void OnRead(RiftClient From)
        {
            Log.Success("Authentification", "Email = " + Email + " SessionTicket = " + SessionTicket);

            long Char = Program.Maps.GetConnecting(Email);

            if (Char == 0)
            {
                Log.Error("Authentification", "Not authentified email : " + Email);
                From.Disconnect();
                return;
            }

            From.Acct = Program.Accounts.GetAccountByUsername(Email);

            if (From.Acct == null)
            {
                Log.Error("Authentification", "Not valid account :" + Email);
                From.Disconnect();
                return;
            }

            From.Character = Program.Characters.GetCharacter(Char);

            if (From.Acct == null)
            {
                Log.Error("Authentification", "Not valid character :" + Char);
                From.Disconnect();
                return;
            }

            WorldAuthenticationResponse Rp = new WorldAuthenticationResponse();
            Rp.AddField(0, EPacketFieldType.True, (bool)true);
            From.SendSerialized(Rp);

            CacheTemplate[] Tmps = Program.World.GetTemplates();
            foreach (CacheTemplate Tmp in Tmps)
                From.SendSerialized(WorldMgr.BuildCache(Tmp.CacheID, Tmp.CacheType, Tmp));

            CacheData[] Dts = Program.World.GetDatas();
            foreach (CacheData Tmp in Dts)
                From.SendSerialized(WorldMgr.BuildCache(Tmp.CacheID, Tmp.CacheType, Tmp));

            WorldCacheUpdated Updated = new WorldCacheUpdated();
            Updated.GUID = Char;
            From.SendSerialized(Updated);

            /////////////////////////////////////////////////////////////////////
            // Send Inventory
            /////////////////////////////////////////////////////////////////////

            WorldEntityUpdate Update = new WorldEntityUpdate();
            Update.AddField(6, EPacketFieldType.Raw8Bytes, (long)Char);
            From.SendSerialized(Update);

            //////////////////////////////////////////////////////////////////////

            
            ISerializablePacket Packet1 = new ISerializablePacket();
            Packet1.Opcode = 0x03F6;
            Packet1.AddField(0, EPacketFieldType.Raw4Bytes, new byte[4] { 0x20, 0xB1, 0x59, 0x41 });
            Packet1.AddField(1, EPacketFieldType.ByteArray, new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                                                                             00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                                                                             00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });

            byte[] UnkGuid = new byte[8] { 0xCB, 0x34, 0x3D, 0x94, 0x23, 0x04, 0xCC, 0x01 };
            Packet1.AddField(2, EPacketFieldType.Raw8Bytes, UnkGuid);
            From.SendSerialized(Packet1);

            ISerializablePacket Packet2 = new ISerializablePacket();
            Packet2.Opcode = 0x02E9;
            Packet2.AddField(0, EPacketFieldType.List, new List<long>() { 3605869292 });
            From.SendSerialized(Packet2);

            ISerializablePacket Packet3 = new ISerializablePacket();
            Packet3.Opcode = 0x2D7F;
            From.SendSerialized(Packet3);

            /********************************************/

            WorldZoneInfo ZoneInfo = new WorldZoneInfo();
            ZoneInfo.ZoneFileName = "Mathosia1";
            ZoneInfo.Description = Program.World.GetText(290412351);
            ZoneInfo.DisplayName = Program.World.GetText(1647389394);
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
