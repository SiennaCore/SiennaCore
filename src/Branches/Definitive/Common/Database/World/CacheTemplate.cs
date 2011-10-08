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
    [DataTable(DatabaseName = "World", TableName = "CacheTemplate", PreCache = true)]
    [ISerializableAttribute((long)Opcodes.CacheTemplate)]
    public class CacheTemplate : ISerializablePacket
    {
        [PrimaryKey()]
        public uint CacheID;

        [DataElement()]
        public long CacheType;
        
        [DataElement()]
        [ArrayBit(0)]
        public string Name;

        [DataElement()]
        [ArrayBit(1)]
        public string Description;

        [DataElement()]
        [Unsigned7Bit(2)]
        public long Field2;

        [DataElement()]
        [Unsigned7Bit(3)]
        public long Field3;

        [DataElement()]
        [Unsigned7Bit(4)]
        public long Field4;

        [DataElement()]
        public byte[] Field5
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field5List");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field5List");
            }
        }

        [ListBit(5)]
        public List<ISerializablePacket> Field5List;

        [DataElement()]
        [ListBit(7)]
        public List<long> Field7;

        [DataElement()]
        public byte[] Field9
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field9Packet");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field9Packet");
            }
        }

        [PacketBit(9)]
        public ISerializablePacket Field9Packet;

        [DataElement()]
        [Unsigned7Bit(10)]
        public long Field10;

        [DataElement()]
        [BoolBit(13)]
        public bool Field13;

        [DataElement()]
        [Unsigned7Bit(14)]
        public long Field14;

        [DataElement()]
        public byte[] Field15
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field15List");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field15List");
            }
        }

        [ListBit(15)]
        public List<ISerializablePacket> Field15List;

        [DataElement()]
        [BoolBit(19)]
        public long Field19;

        [DataElement()]
        [Unsigned7Bit(20)]
        public long Field20;

        [DataElement()]
        [Unsigned7Bit(21)]
        public long Field21;

        [DataElement()]
        [Unsigned7Bit(22)]
        public long Field22;

        [DataElement()]
        [Unsigned7Bit(34)]
        public long Field34;

        [DataElement()]
        public byte[] Field37
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field37List");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field37List");
            }
        }

        [ListBit(37)]
        public List<ISerializablePacket> Field37List;

        [DataElement()]
        public long TextID
        {
            get
            {
                if (Field40 != null)
                    return Field40.ID;
                else
                    return 0;
            }
            set
            {
                Field40 = new TextInfo();
                Field40.ID = value;
            }
        }

        [PacketBit(40)]
        public TextInfo Field40;

        [DataElement()]
        public byte[] Field41
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field41Packet");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field41Packet");
            }
        }

        [PacketBit(41)]
        public ISerializablePacket Field41Packet;

        [DataElement()]
        [BoolBit(51)]
        public bool Field51 = false;

        [DataElement()]
        [BoolBit(73)]
        public bool Field73 = false;

        [DataElement()]
        [BoolBit(75)]
        public bool Field75 = false;
    }
}
