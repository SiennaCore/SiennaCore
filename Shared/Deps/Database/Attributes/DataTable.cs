using System;

namespace Shared.Database
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTable : Attribute
    {
        public DataTable()
        {
            TableName = null;
            PreCache = false;
        }

        // Défini le nom de la table a charger
		// Set the table name to load
        public string TableName { get; set; }

        // Pour l'affichage de la table
		// To display the table
        public string ViewName { get; set; }

        // Nom de la DB a utiliser
		// Name of DB to use
        public string DatabaseName { get; set; }

        // True si la table doit être préchargée pour optimiser les performances
		// True if the table should be preloaded to optimize performance
        public bool PreCache { get; set; }
    }
}
