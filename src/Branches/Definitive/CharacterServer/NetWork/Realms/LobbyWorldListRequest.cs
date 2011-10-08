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
    [ISerializableAttribute((long)Opcodes.LobbyWorldListRequest)]
    public class LobbyWorldListRequest : ISerializablePacket
    {
        public override void OnRead(RiftClient From)
        {
            Log.Success("WorldList", "Request : In Progress");

            Realm[] Realms = Program.AcctMgr.GetRealms();

            LobbyWorldListResponse Rp = new LobbyWorldListResponse();
            foreach(Realm Rm in Realms)
            {
                LobbyWorldEntry Entry = new LobbyWorldEntry();
                Entry.RealmID = Rm.RiftId;
                Entry.PVP = Rm.PVP == 1;
                Entry.Recommended = Rm.Recommended == 1;
                Entry.Population = 0;
                Entry.RP = Rm.RP == 1;
                Entry.Version = Rm.ClientVersion;
                Entry.CharactersCount = Rm.GetObject<CharactersMgr>().GetCharactersCount(From.Acct.Id);
                Entry.Language = Rm.Language;
                if (Rm.Online > 0)
                    Entry.AddField(16, EPacketFieldType.True, (bool)true);

                Rp.Realms.Add(Entry);
            }

            Log.Success("WorldList", "Count = " + Rp.Realms.Count);

            From.SendSerialized(Rp);
        }
    }
}
