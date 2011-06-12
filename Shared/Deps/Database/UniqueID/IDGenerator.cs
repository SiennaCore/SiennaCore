using System;

namespace Shared.Database
{
    public class IDGenerator
    {
        // Génère un ID unique pour chaque objet
        // Generates a unique ID for each
        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}