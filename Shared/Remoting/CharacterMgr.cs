using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

namespace Shared
{
    [RpcAttributes(new string[] { "CharacterServer" })]
    public class CharacterMgr : ARpc
    {
        static public ObjectDatabase CharacterDB = null;

        #region Realms

        static public Dictionary<int, Realm> _Realms = new Dictionary<int, Realm>();

        public void LoadRealms()
        {
            _Realms.Clear();

            IList<Realm> Rms = CharacterDB.SelectAllObjects<Realm>();

            foreach (Realm Rm in Rms)
            {
                Rm.GenerateName();
                Rm.Dirty = true;

                _Realms.Add(Rm.RealmId, Rm);
                CharacterDB.SaveObject(Rm);
            }

            Log.Succes("LoadRealms", "Loaded " + _Realms.Count + " Realm(s)");
        }
        public Realm GetRealm(byte RealmId)
        {
            return GetRealms().ToList().Find(info => info.RealmId == RealmId);
        }
        public Realm GetRealm(int RiftId)
        {
            return GetRealms().ToList().Find(info => info.RiftId == RiftId);
        }
        public Realm[] GetRealms()
        {
            return _Realms.Values.ToArray();
        }

        #endregion

        #region Characters

        public Character GetCharacter(int CharacterId)
        {
            return CharacterDB.SelectObject<Character>("Id='" + CharacterId + "'");
        }
        public Character GetCharacter(string Name)
        {
            return CharacterDB.SelectObject<Character>("Name='" + CharacterDB.Escape(Name) + "'");
        }
        public Character GetCharacter(string Name, byte RealmId)
        {
            return CharacterDB.SelectObject<Character>("Name='" + CharacterDB.Escape(Name) + "' AND RealmId='" + RealmId + "'");
        }

        public Character[] GetCharacters(int AccountId,byte RealmId)
        {
            return CharacterDB.SelectObjects<Character>("AccountId='" + AccountId + "' AND RealmId='" + RealmId + "'").ToArray();
        }
        public Character[] GetCharacters(byte RealmId)
        {
            return CharacterDB.SelectObjects<Character>("RealmId='" + RealmId + "'").ToArray();
        }
        public Character[] GetCharacters(string Name)
        {
            return CharacterDB.SelectObjects<Character>("Name='" + CharacterDB.Escape(Name) + "'").ToArray();
        }

        public void AddCharacter(Character Char)
        {
            CharacterDB.AddObject(Char);
        }
        public void SaveCharacter(Character Char)
        {
            Char.Dirty = true;
            CharacterDB.SaveObject(Char);
        }
        public void DeleteCharacter(Character Char)
        {
            CharacterDB.DeleteObject(Char);
        }

        #endregion
    }
}
