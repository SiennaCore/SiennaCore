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
        public Realm[] GetRealms()
        {
            return _Realms.Values.ToArray();
        }
        public Realm GetRealm(int RiftId)
        {
            return GetRealms().ToList().Find(info => info.RiftId == RiftId);
        }

        #endregion

        #region Characters


        #endregion
    }
}
