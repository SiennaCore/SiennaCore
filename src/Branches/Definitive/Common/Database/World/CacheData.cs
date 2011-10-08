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
using System.Reflection;

using FrameWork;

namespace Common
{
    [Serializable]
    [ISerializableAttribute((long)Opcodes.CacheReference)]
    public class CacheReference : ISerializablePacket
    {
        [Raw4Bit(0)]
        public uint CacheID;
    }

    [Serializable]
    [DataTable(DatabaseName = "World", TableName = "CacheData", PreCache = true)]
    [ISerializableAttribute((long)Opcodes.CacheData)]
    public class CacheData : ISerializablePacket
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
        public string Container;

        [DataElement()]
        [ArrayBit(2)]
        public string Description;

        /*[DataElement()]
        public long Field4I0
        {
            set
            {
                if (value == 0)
                    return;

                ISerializablePacket Packet = new ISerializablePacket();
                Packet.Opcode = 0x1C9D;
                Packet.AddField(0, EPacketFieldType.Unsigned7BitEncoded, value);
                Field4.Add(Packet);
            }
            get
            {
                ISerializablePacket Packet = Field4.Find(info => info.Opcode == 0x1C9D);
                return Field4.Count;

                if (Packet == null || Packet.GetField(0) == null)
                    return 0;
                else
                    return Packet.GetField(0).GetUint();
            }
        }

        [DataElement()]
        public long ReferenceID
        {
            get
            {
                if (Reference != null)
                    return Reference.CacheID;
                else
                    return 0;
            }
            set
            {
                if (value == 0)
                    return;

                    CacheReference Ref;
                    if (Reference != null)
                        Ref = Reference;
                    else
                        Ref = new CacheReference();

                    Ref.CacheID = value;
            }
        }

        public CacheReference Reference
        {
            get
            {
                ISerializablePacket Packet = Field4.Find(info => info.Opcode == 0x1C9C);
                if (Packet == null)
                    return null;

                return Packet as CacheReference;
            }
        }*/

        [DataElement()]
        public byte[] Field4
        {
            get
            {
                return PacketProcessor.FieldToBytes(this, "Field4List");
            }
            set
            {
                PacketProcessor.BytesToField(this, value, "Field4List");
            }
        }

        [ListBit(4)]
        public List<ISerializablePacket> Field4List;

        [DataElement()]
        [Unsigned7Bit(5)]
        public long Field5;

        [DataElement()]
        public long TextID_1
        {
            get
            {
                if (Field7 != null)
                    return Field7.ID;
                else
                    return 0;
            }
            set
            {
                Field7 = new TextInfo();
                Field7.ID = value;
            }
        }

        [PacketBit(7)]
        public TextInfo Field7;

        [DataElement()]
        public long TextID_2
        {
            get
            {
                if (Field8 != null)
                    return Field8.ID;
                else
                    return 0;
            }
            set
            {
                Field8 = new TextInfo();
                Field8.ID = value;
            }
        }

        [PacketBit(8)]
        public TextInfo Field8;
    }
}
