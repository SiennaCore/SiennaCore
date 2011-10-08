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

using FrameWork;

namespace Common
{
    [Serializable]
    [DataTable(DatabaseName = "Characters", TableName = "CharactersInfo", PreCache = false)]
    [ISerializableAttribute((long)Opcodes.LobbyCharacterInfo)]
    public class CharacterInfo : ISerializablePacket
    {
        [DataElement()]
        public long CharacterId;

        [DataElement()]
        [Unsigned7Bit(1)]
        public long Race;

        [DataElement()]
        [Unsigned7Bit(2)]
        public long Sex;

        [DataElement()]
        [Unsigned7Bit(4)]
        public uint MapID = 290412351;

        [DataElement()]
        [Raw4Bit(5)]
        public uint Field5 = 777065106;

        [DataElement()]
        [Raw4Bit(6)]
        public uint Field6 = 3;

        [DicBit(7)]
        public Dictionary<long, ISerializablePacket> Field7 = new Dictionary<long, ISerializablePacket>(); // Models

        [DataElement()]
        public byte[] Equipements
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field7");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field7");
            }
        }

        [DataElement()]
        [Unsigned7Bit(8)]
        public long Field8 = 10;

        [DataElement()]
        [Unsigned7Bit(9)]
        public long Level;

        [DataElement()]
        [Unsigned7Bit(15)]
        public long Class;

        [DataElement()]
        [Unsigned7Bit(16)]
        public long Field16 = 15;

        [DataElement()]
        [Raw4Bit(17)]
        public uint Field17 = 1712948866;

        [DataElement()]
        [ArrayBit(18)]
        public string MapName = "guardian_map";

        [Unsigned7Bit(19)]
        public long Field19 = 177;

        [PacketBit(21)]
        public LobbyCharacterCustom CustomPacket;

        [DataElement()]
        public byte[] Custom
        {
            get
            {
                byte[] Bts = PacketProcessor.FieldToBytes(this, "CustomPacket");
                return Bts;
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "CustomPacket");
            }
        }

        [DataElement()]
        [ListBit(24)]
        public List<uint> Field24 = new List<uint>(new uint[] { 1149965263, 1147537107, 1152778324 });

        [ListBit(25)]
        public List<ISerializablePacket> Field25List = new List<ISerializablePacket>();

        [DataElement()]
        public byte[] Field25
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field25List");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field25List");
            }
        }

        [DataElement()]
        [Unsigned7Bit(26)]
        public long Field26 = 60006;
    }
}
