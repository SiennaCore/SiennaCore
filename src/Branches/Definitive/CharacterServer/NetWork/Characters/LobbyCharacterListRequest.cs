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
    [ISerializableAttribute((long)Opcodes.LobbyCharacterListRequest)]
    public class LobbyCharacterListRequest : ISerializablePacket
    {
        public override void OnRead(RiftClient From)
        {
            Log.Success("CharacterListRequest", "Characters For : " + From.GetIp + " RPC : " + From.Rm.RpcInfo.Description());

            if (From.Acct == null || From.Rm == null)
                return;

            LobbyCharacterListResponse ListRp = new LobbyCharacterListResponse();
            Character[] Chars = From.Rm.GetObject<CharactersMgr>().GetCharacters(From.Acct.Id);
            foreach (Character Char in Chars)
                ListRp.Characters.Add(Char);
            From.SendSerialized(ListRp);

            Log.Success("Characters","Count = " + ListRp.Characters.Count);
        }
    }
}
