using System;

namespace Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class Relation : Attribute
    {
        public Relation()
        {
            LocalField = null;
            RemoteField = null;
            AutoLoad = true;
            AutoDelete = false;
        }
        
        // Relation entre plusieurs champs de table différente
        // Relationship between several different table fields
        public string LocalField { get; set; }

        // Supprime la relation
        // Removes the relationship
        public string RemoteField { get; set; }

        // Chargement automatique de la table
        // Automatic loading table
        public bool AutoLoad { get; set; }

        // Suppression de l'objet automatiquement lorsque l'objet est supprimé dans le core
        // Deleting the object automatically when the object is removed from the core
        public bool AutoDelete { get; set; }
    }
}
