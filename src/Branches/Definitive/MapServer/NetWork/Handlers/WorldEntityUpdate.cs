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

using Common;
using FrameWork;

namespace MapServer
{
    [ISerializableAttribute((long)Opcodes.WorldEntityUpdate)]
    public class WorldEntityUpdate : ISerializablePacket
    {
        [Raw8Bit(0)]
        public long GUID;

        [ListBit(1)]
        public List<ISerializablePacket> Field1;

        [Unsigned7Bit(4)]
        public long Field4;

        [BoolBit(5)]
        public bool Field5 = true;

        [BoolBit(7)]
        public bool Field7 = true;

        public ISerializablePacket GetPacketOnList(long Opcode)
        {
            if (Field1 != null)
                return Field1.Find(info => info.Opcode == Opcode);

            return null;
        }

        public bool Build(Character Char)
        {
            if (Char == null)
                return false;

            if (Field1 == null)
                Field1 = new List<ISerializablePacket>();

            GUID = Char.CharacterId;
            Field4 = 1001;

            ISerializablePacket UnkPck = new ISerializablePacket();
            UnkPck.Opcode = 0x0259;
            UnkPck.AddField(3, EPacketFieldType.Raw4Bytes, (float)50.0f);

            SetCharacterInformation(Char);
            SetCharacterStats(Char);
            SetPosition(676, "guardian_map", new List<float> { 1113.03967f, 920.1114f, 1444.58533f });

            AddPacketToList(0x026B);
            AddPacketToList(0x025A);
            AddPacketToList(0x026E);
            AddPacketToList(0x0272);
            AddPacketToList(0x0289);
            AddPacketToList(0x028A);

            // Another Social Junk ?
            ISerializablePacket UnkPck5 = new ISerializablePacket();
            UnkPck5.Opcode = 0x028F;
            ISerializablePacket UnkPck5Pck = new ISerializablePacket();
            UnkPck5Pck.Opcode = 0x0354;
            UnkPck5Pck.AddField(0, EPacketFieldType.Unsigned7BitEncoded, (long)2);
            UnkPck5.AddField(4, EPacketFieldType.Packet, UnkPck5Pck);
            Field1.Add(UnkPck5);

            AddPacketToList(0x033A);
            AddPacketToList(0x08A2);
            AddPacketToList(0x08FC);
            AddPacketToList(0x0F41);
            AddPacketToList(0x0F45);
                

            return true;
        }

        public void SetPosition(long MapID, string MapName, List<float> Position)
        {
            WorldPositionExtra WorldPos = GetPacketOnList((long)Opcodes.WorldPositionExtra) as WorldPositionExtra;
            WorldPos.MapId = MapID;
            WorldPos.Position = Position; // X,Y,Z
            WorldPos.MapName = MapName;
        }

        public void SetCharacterInformation(Character Char)
        {
            ISerializablePacket CharacterInformation = GetPacketOnList((long)Opcodes.WorldCharacterInformation);
            CharacterInformation.AddField(12, EPacketFieldType.ByteArray, Char.CharacterName);
            CharacterInformation.AddField(54, EPacketFieldType.Unsigned7BitEncoded, Program.Rm.RiftId);
            CharacterInformation.AddField(56, EPacketFieldType.Packet, Char.Info.CustomPacket);
        }

        public void SetCharacterStats(Character Char)
        {
            ISerializablePacket Packet = GetPacketOnList(0x0294);
            Packet.AddField(11, EPacketFieldType.Dictionary, new Dictionary<long, ISerializablePacket>());
        }

        public void RemovePacketFromList(long Opcode)
        {
            Field1.RemoveAll(info => info.Opcode == Opcode);
        }

        public void AddPacketToList(long Opcode)
        {
            RemovePacketFromList(Opcode);

            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = Opcode;
            Field1.Add(Packet);
        }

    }
}
