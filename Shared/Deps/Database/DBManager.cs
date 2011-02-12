using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Shared.Database
{
    // Classe permettant facilement d'assembler les tables et de lancer une connexion
    public class DBManager
    {

        private readonly FileInfo _file = new FileInfo("sql.conf");

        /*public static MySQLObjectDatabase Start(FileInfo configfile, ConnectionType Type, string DBName)
        {
            Log.Info("IObjectDatabase", "Starting...");
            XMLConfigFile xmlConfig = XMLConfigFile.ParseXMLFile(configfile);
            if (xmlConfig == null)
            {
                Log.Error("IOBjectDatabase", "Invalid file : " + configfile);
                return null;
            }


            string sqlconfig = xmlConfig["Database"]["DBInfo"].GetString("");

            if (sqlconfig.Length <= 0)
            {
                Log.Error("IOBjectDatabase", "Invalid configuration : " + configfile);
                return null;
            }

            return Start(sqlconfig, Type, DBName);
        }*/

        public static MySQLObjectDatabase Start(string sqlconfig, ConnectionType Type, string DBName)
        {
            Log.Debug("IObjectDatabase", DBName + "->Start " + sqlconfig + "...");
            IObjectDatabase _database = null;

            if (_database == null)
            {

                _database = ObjectDatabase.GetObjectDatabase(Type, sqlconfig);

                try
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.IsClass != true)
                                continue;

                            DataTable[] attrib = (DataTable[])type.GetCustomAttributes(typeof(DataTable), true);
                            if (attrib.Length > 0 && attrib[0].DatabaseName == DBName)
                            {
                                Log.Info("DBManager", "Registering table: " + type.FullName);
                                _database.RegisterDataObject(type);
                            }
                        }
                    }
                }
                catch (DatabaseException e)
                {
                    Log.Error("DBManager", "Error at registering tables " + e.ToString());
                    return null;
                }
            }
               return (MySQLObjectDatabase)_database;
        }

    }
}
