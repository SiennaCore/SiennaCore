using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace Sienna.Game
{
    public class DatabaseInformations
    {
        public string Address = "127.0.0.1";
        public int Port = 3306;
        public string DatabaseName = "Sienna-Logon";
        public string Username = "Sienna";
        public string Password = "Sienna";
    }

    public class LogonConfig
    {
        private static LogonConfig lconfig;
        public static LogonConfig get
        {
            get
            {
                if(lconfig == null)
                    lconfig = Configuration<LogonConfig>.Load("Logon.conf");

                return lconfig;
            }
        }

        public int LoginPort = 6900;
        public int SocketThreads = 4;
        public DatabaseInformations LoginDatabase = new DatabaseInformations();

        public string LogFile = "Logon.log";

        public Log.LogLevel ConsoleLogLevel = Log.LogLevel.Info;
        public Log.LogLevel FileLogLevel = Log.LogLevel.Info;

        public int DatabaseThreads = 4;

        public int IntercomPort = 8859;
        public string IntercomKey = "Important-Change-Me";

        public bool UsingCustomCertificateServer = false;
    }
}
