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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterSelectResponse)]
    public class LobbyCharacterSelectResponse : ISerializablePacket
    {
        [ListBit(1)]
        public List<string> Ips = new List<string>();
    }

    [ISerializableAttribute((long)Opcodes.LobbyCharacterSelectRequest)]
    public class LobbyCharacterSelectRequest : ISerializablePacket
    {
        [Unsigned7Bit(0)]
        public long GUID;

        public override void OnRead(RiftClient From)
        {
            Log.Success("SelectRequest", "Enter on World : " + From.GetIp + ",GUID=" + GUID);

            if (From.Acct == null || From.Rm == null)
                return;

            Character Char = From.Rm.GetObject<CharactersMgr>().GetCharacter(GUID);
            if (Char == null || Char.AccountId != From.Acct.Id)
            {
                Log.Error("SelectRequest", "Invalid CharacterId = " + GUID);
                From.Disconnect();
                return;
            }


            MapServerInfo Info = From.Rm.GetObject<WorldMgr>().GetMapInfo();

            if (Info == null)
            {
                Log.Error("SelectRequest", "No map loaded ! Run MapServer");
                return;
            }

            Log.Success("SelectRequest", "Entering on Map : " + Info.MapAdress);

            Info.GetObject<MapMgr>().RegisterConnecting(From.Acct.Username,Char.CharacterId);

            LobbyCharacterSelectResponse Rp = new LobbyCharacterSelectResponse();
            Rp.Ips.Add(Info.MapAdress);
            From.SendSerialized(Rp);

        }
    }
}
