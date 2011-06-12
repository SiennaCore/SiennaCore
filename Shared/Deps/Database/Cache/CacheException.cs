using System;

namespace Shared.Database
{
    public class CacheException : DatabaseException
    {
        // Construit un exeption levée par le cache
        // Constructs a exeption thrown by the cache
        public CacheException(string s)
            : base(s)
        {
        }

        // Description de l'exeption levée
        // Constructs a exeption thrown by the cache
        public CacheException(Exception e)
            : base(e)
        {
        }
    }
}