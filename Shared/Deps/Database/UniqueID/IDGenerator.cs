using System;

namespace Shared.Database
{
    public class IDGenerator
    {
        // Génère un ID unique pour chaque objet
        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}