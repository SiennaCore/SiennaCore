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
    [Rpc(false, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 1)]
    public class CharactersMgr : RpcObject
    {
        static public MySQLObjectDatabase CharactersDB;

        public int StartingLevel = 1;
        public long MaxCharacterId = 10;

        public void Load()
        {
            LoadRandomNames();
            LoadMaxCharacterId();
        }

        #region Realm

        static public Realm MyRealm;
        static public RpcClient Client;

        public override void OnServerConnected()
        {
            if(MyRealm != null && Client != null)
                Client.GetServerObject<AccountMgr>().RegisterRealm(MyRealm, Client.Info);
        }

        public Realm GetRealm()
        {
            return MyRealm;
        }

        public AccountMgr GetAccounts()
        {
            return Client.GetServerObject<AccountMgr>();
        }

        #endregion

        #region Characters

        public void LoadMaxCharacterId()
        {
            Character Char = CharactersDB.SelectObject<Character>("1=1 ORDER BY `CharacterId` DESC LIMIT 0, 1");
            if (Char != null)
                MaxCharacterId = Char.CharacterId;
        }

        public Character[] GetCharacters(long AccountId)
        {
            return CharactersDB.SelectObjects<Character>("AccountId=" + AccountId).ToArray();
        }

        public int GetCharactersCount(long AccountId)
        {
            return CharactersDB.GetObjectCount<Character>("AccountId=" + AccountId);
        }

        public bool CharacterExist(string Name)
        {
            return CharactersDB.GetObjectCount<Character>("CharacterName='" + CharactersDB.Escape(Name) + "'") > 0;
        }

        public Character GetCharacter(string Name)
        {
            return CharactersDB.SelectObject<Character>("CharacterName='" + CharactersDB.Escape(Name) + "'");
        }

        public Character GetCharacter(long CharacterId)
        {
            return CharactersDB.SelectObject<Character>("CharacterId=" + CharacterId);
        }

        public LobbyCharacterListResponse GetCharactersList(long AccountId)
        {
            Character[] Chars = GetCharacters(AccountId);

            LobbyCharacterListResponse Rp = new LobbyCharacterListResponse();
            Rp.Characters.AddRange(Chars);
            return Rp;
        }

        public List<ISerializablePacket> GetCreationCache()
        {
            List<ISerializablePacket> Caches = new List<ISerializablePacket>();

            return Caches;

        }

        public void AddCharacter(Character Char)
        {
            long CharacterId = System.Threading.Interlocked.Increment(ref MaxCharacterId);

            Log.Success("AddCharacter", "Creating New Character : " + CharacterId + ", Name = " + Char.CharacterName);

            Char.CharacterId = CharacterId;
            Char.Info.CharacterId = CharacterId;

            CharactersDB.AddObject(Char);
            CharactersDB.AddObject(Char.Info);
        }

        #endregion

        #region Creation

        public List<RandomName> RandomNames;

        public void LoadRandomNames()
        {
            RandomNames = new List<RandomName>();

            RandomNames.AddRange(CharactersDB.SelectAllObjects<RandomName>());

            Log.Success("LoadRandomNames", "" + RandomNames.Count + " : Random Names loaded");
        }

        public string GetRandomName()
        {
            if (RandomNames != null && RandomNames.Count > 0)
                for (int TryCount = 0; TryCount < 10; ++TryCount)
                {
                    int ID = RandomMgr.Next(0, RandomNames.Count - 1);
                    if (GetCharacter(RandomNames[ID].Name) == null)
                        return RandomNames[ID].Name;
                }

            return "RandomName";
        }

        #endregion
    }
}
