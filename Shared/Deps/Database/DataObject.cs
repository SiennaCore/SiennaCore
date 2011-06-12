using System;
using System.ComponentModel;

namespace Shared.Database
{
    // Classe de base de tous les DataObject
    // Base class of all DataObject
    [Serializable]
    public abstract class DataObject : ICloneable
    {
        bool m_allowAdd = true;
        bool m_allowDelete = true;

        // Génération d'un objet unique pour chaque DataObject
        // Generating a unique object for each DataObject
        protected DataObject()
        {
            ObjectId = IDGenerator.GenerateID();
            IsValid = false;
            AllowAdd = true;
            AllowDelete = true;
            IsDeleted = false;
        }

        // Nom de la table dont l'objet provient
        // Table name from which the object
        [Browsable(false)]
        public virtual string TableName
        {
            get
            {
                Type myType = GetType();
                return GetTableName(myType);
            }
        }

        // Chargement en cache ou non de l'objet
        // Loading cached or otherwise of the object
        [Browsable(false)]
        public virtual bool UsesPreCaching
        {
            get
            {
                Type myType = GetType();
                return GetPreCachedFlag(myType);
            }
        }

        // Objet Valide ?
        // Valid object?
        [Browsable(false)]
        public bool IsValid { get; set; }

        // Peut être ou non ajouté a la DB
        // May or may not be added to the DB
        [Browsable(false)]
        public virtual bool AllowAdd
        {
            get { return m_allowAdd; }
            set { m_allowAdd = value; }
        }

        // Peut être ou non supprimé de la DB
        // May or may not be added to the DB
        [Browsable(false)]
        public virtual bool AllowDelete
        {
            get { return m_allowDelete; }
            set { m_allowDelete = value; }
        }

        // Numéro de l'objet dans la table
        // Item number in the table
        [Browsable(false)]
        public string ObjectId { get; set; }

        // Objet différent ke celui de la table ?
        // Item number in the table
        [Browsable(false)]
        public virtual bool Dirty { get; set; }

        // Cette objet a été delete de la table ?
        // This object has been deleted from the table?
        [Browsable(false)]
        public virtual bool IsDeleted { get; set; }


        #region ICloneable Member

        // Créer un clone de l'objet
        // Create a clone of the object
        public object Clone()
        {
            var obj = (DataObject)MemberwiseClone();
            obj.ObjectId = IDGenerator.GenerateID();
            return obj;
        }

        #endregion

        // Récupère la table name en lisant les attributs
        // Retrieves the table name by reading the attributes
        public static string GetTableName(Type myType)
        {
            object[] attri = myType.GetCustomAttributes(typeof(DataTable), true);

            if ((attri.Length >= 1) && (attri[0] is DataTable))
            {
                var tab = attri[0] as DataTable;
                string name = tab.TableName;
                if (name != null)
                    return name;
            }

            return myType.Name;
        }

        public static string GetViewName(Type myType)
        {
            object[] attri = myType.GetCustomAttributes(typeof(DataTable), true);

            if ((attri.Length >= 1) && (attri[0] is DataTable))
            {
                var tab = attri[0] as DataTable;
                string name = tab.ViewName;
                if (name != null)
                    return name;
            }

            return null;
        }

        // Précache au démarrage ?
        // Precache start
        public static bool GetPreCachedFlag(Type myType)
        {
            object[] attri = myType.GetCustomAttributes(typeof(DataTable), true);
            if ((attri.Length >= 1) && (attri[0] is DataTable))
            {
                var tab = attri[0] as DataTable;
                return tab.PreCache;
            }

            return false;
        }
    }
}