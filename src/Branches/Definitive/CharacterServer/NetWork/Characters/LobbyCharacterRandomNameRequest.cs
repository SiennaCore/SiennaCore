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

namespace CharacterServer
{
    [ISerializableAttribute((long)Opcodes.LobbyCharacterRandomNameRequest)]
    public class LobbyCharacterRandomNameRequest : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long Race;

        [BoolBit(0)]
        public bool Sex;

        [Unsigned7Bit(2)]
        public long Faction;

        [Unsigned7Bit(3)]
        public long Field3;

        [Unsigned7Bit(4)]
        public long Class;

        public override void OnRead(RiftClient From)
        {
            if(From.Acct == null || From.Rm == null)
                return;

            string Name = From.Rm.GetObject<CharactersMgr>().GetRandomName();

            ISerializablePacket Packet = new ISerializablePacket();
            Packet.Opcode = (int)Opcodes.LobbyCharacterRandomNameResponse;
            Packet.AddField(1, EPacketFieldType.ByteArray, Name);
            From.SendSerialized(Packet);
        }
    }
}
