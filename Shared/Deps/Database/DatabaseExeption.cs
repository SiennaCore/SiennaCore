using System;

namespace Shared.Database
{
    public class DatabaseException : ApplicationException
    {
        // Exeption levé par la database
        // Exeption raised by the database
        public DatabaseException(Exception e)
            : base("", e)
        {
        }

        // Lèvre un exeption avec le message d'erreur
        // lip with an exeption error
        public DatabaseException(string str, Exception e)
            : base(str, e)
        {
        }

        // Raisons de l'exeption
        // Reasons of the exeption
        public DatabaseException(string str)
            : base(str)
        {
        }
    }
}