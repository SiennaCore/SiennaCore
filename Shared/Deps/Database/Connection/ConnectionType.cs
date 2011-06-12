
namespace Shared.Database
{
    // Liste des connexions supportées
	// List of connections supported
    public enum ConnectionType
    {
        // Mysql Serveur
		// Mysql Server
        DATABASE_MYSQL,

        // MicroSoft SQL Serveur
		// MicroSoft SQL Server
        DATABASE_MSSQL,

       // ODBC database serveur
       // ODBC database server
        DATABASE_ODBC,

        // OLE database serveur
		// OLE database server
        DATABASE_OLEDB
    }
}