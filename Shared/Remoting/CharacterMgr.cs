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

            Log.Success("LoadRealms", "Loaded " + _Realms.Count + " Realm(s)");
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

        public Character[] GetCharacters(long AccountId,byte RealmId)
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

        public void AddObject(DataObject Char)
        {
            CharacterDB.AddObject(Char);
        }
        public void SaveObject(DataObject Char)
        {
            Char.Dirty = true;
            CharacterDB.SaveObject(Char);
        }
        public void RemoveObject(DataObject Char)
        {
            CharacterDB.DeleteObject(Char);
        }

        static public List<RandomName> _Randoms = new List<RandomName>();

        public void LoadRandomNames()
        {
            _Randoms.AddRange(CharacterDB.SelectAllObjects<RandomName>());

            Log.Success("LoadRandomNames", "Loaded " + _Randoms.Count + " Random Name(s)");
        }

        public string GenerateName()
        {
            if (_Randoms.Count <= 0)
                return "Sienna";

            Random R = new Random();
            int Id = R.Next(_Randoms.Count);

            return _Randoms[Id].Name;
        }

        #endregion
    }
}
