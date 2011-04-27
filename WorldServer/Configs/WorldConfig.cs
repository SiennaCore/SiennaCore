using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace WorldServer
{
    public class DatabaseInfo
    {
        public string Server = "127.0.0.1";
        public string Port = "3306";
        public string Database = "databasename";
        public string Username = "root";
        public string Password = "password";
        public string Custom = "Treat Tiny As Boolean=False";

        public string Total()
        {
            string Result = "";
            Result += "Server=" + Server + ";";
            Result += "Port=" + Port + ";";
            Result += "Database=" + Database + ";";
            Result += "User Id=" + Username + ";";
            Result += "Password=" + Password + ";";
            Result += Custom;
            return Result;
        }
    }

    [aConfigAttributes("Configs/World.xml")]
    public class WorldConfig : aConfig
    {
        public string ServerMOTD = "Welcome to SiennaCore server";

        public string WorldServerIP = "127.0.0.1";
        public int WorldServerPort = 6901;

        public string RpcIp = "127.0.0.1";
        public int RpcPort = 6899;
        public string RpcKey = "password";

        public byte RealmId = 1;

        public DatabaseInfo WorldDB = new DatabaseInfo();

        [aConfigMethod()]
        static public void OnLoad(aConfigAttributes Attributes, aConfig Conf, bool FirstLoad)
        {
            if (FirstLoad || !Conf.IConfiguredTheFile)
            {
                if (FirstLoad)
                    Log.Info("Config", "This is your first launch.");
                else if (!Conf.IConfiguredTheFile)
                    Log.Info("Config", "IConfiguredTheFile value is false.");

                Log.Info("Config", "A configuration file was created : " + Attributes.FileName);
                Log.Info("Config", "You must configure the server before continuing.");
                Log.Info("Config", "Press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
