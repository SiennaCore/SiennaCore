using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;

namespace CharacterServer
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

    [aConfigAttributes("Configs/Characters.xml")]
    public class CharacterConfig : aConfig
    {
        public int CharacterServerPort = 6900;

        public DatabaseInfo AccountsDB = new DatabaseInfo();
        public DatabaseInfo CharactersDB = new DatabaseInfo();

        public int RpcPort = 6899;
        public string RpcKey = "password";

        public bool UseCertificate = false;
    }
}
