using System;
using System.Collections;

namespace Shared.Database
{
    public class SimpleCache : ICache
    {
        private readonly Hashtable _cache = Hashtable.Synchronized(new Hashtable());

        #region ICache Members

        // Retourne toutes les clef enregitré dans la collection
        // Return all key registered in the collection
        public ICollection Keys
        {
            get { return _cache.Keys; }
        }

        // Ajoute ou récupère un objet a partir de sa clef
        // Return all key stored in the collection
        public object this[object key]
        {
            get
            {
                var wr = _cache[key] as WeakReference;
                if (wr == null || !wr.IsAlive)
                {
                    _cache.Remove(key);
                    return null;
                }

                return wr.Target;
            }
            set
            {
                if (value == null)
                {
                    _cache.Remove(key);
                }
                else
                {
                    _cache[key] = new WeakReference(value);
                }
            }
        }

        #endregion
    }
}