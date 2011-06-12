using System.Collections;

namespace Shared.Database
{
    public interface ICache
    {
        // Collection des Clef du cache
		// Collection Key cache
        ICollection Keys { get; }

        // Récupère un objet de la collection
		// Retrieves a collection object
        object this[object key] { get; set; }
    }
}