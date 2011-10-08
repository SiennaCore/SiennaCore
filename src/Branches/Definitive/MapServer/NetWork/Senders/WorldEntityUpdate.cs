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
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Common;
using FrameWork;

namespace MapServer
{
    [ISerializableAttribute((long)Opcodes.WorldPositionExtra)]
    public class WorldPositionExtra : ISerializablePacket
    {
        [ArrayBit(1)]
        public string MapName;

        [Unsigned7Bit(2)]
        public long MapId = 676;

        [ListBit(3)]
        public List<float> Position = new List<float>() { 1113.03967f, 920.1114f, 1444.58533f }; // X,Y,Z

        // Unk
        [ListBit(4)]
        public List<uint> Field4 = new List<uint>() { 2147483648, 3212777419, 2147483648, 3182182386 }; // Unk

        [Raw4Bit(5)]
        public uint Field5 = 1065688760;

        // Zone Offset ?
        [ListBit(8)]
        public List<float> Position2 = new List<float>() { 1113.03967f, 920.1114f, 1444.58533f };
    }

    [ISerializableAttribute((long)Opcodes.CharacterInfoCache)]
    public class CharacterInfoCache : ISerializablePacket
    {
        [Raw4Bit(0)]
        public uint CacheIdentifier;
    }

    [ISerializableAttribute((long)Opcodes.CharacterInfoDesc)]
    public class CharacterDesc : ISerializablePacket
    {
        [PacketBit(8)]
        public CharacterInfoCache Field8;
    }
}
