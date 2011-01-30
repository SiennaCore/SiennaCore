using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Sienna.Database;
using Sienna.Intercom;

namespace Sienna.Game
{
    public static class LogonMgr
    {
        public static string LogonUrl = "127.0.0.1:8859";
        public static LogonServer LServ;
        public static SQLDatabase LDatabase;
        public static IntercomServer LIntercom;

        public static void Start()
        {
            Log.Init(LogonConfig.get.LogFile, LogonConfig.get.FileLogLevel, LogonConfig.get.ConsoleLogLevel);

            Log.Info(",---.o");                 
            Log.Info("`---..,---.,---.,---.,---.");
            Log.Info("    |||---'|   ||   |,---|");
            Log.Info("`---'``---'`   '`   '`---^ Core");
            Log.Info("");

            LDatabase = new SQLDatabase(LogonConfig.get.LoginDatabase.DatabaseName, LogonConfig.get.LoginDatabase.Address, LogonConfig.get.LoginDatabase.Port, LogonConfig.get.LoginDatabase.Username, LogonConfig.get.LoginDatabase.Password);
            Log.Info("");

            LIntercom = new IntercomServer();
            int Binded = LIntercom.Bind(LogonConfig.get.IntercomPort, LogonConfig.get.IntercomKey);

            Log.Info(">> Sienna Clustering Intercom registered " + Binded + " classes available on port " + LogonConfig.get.IntercomPort);
            Log.Info("");

            LogonUrl = "";

            LServ = new LogonServer(LogonConfig.get.SocketThreads, 50);
            LServ.Bind(LogonConfig.get.LoginPort);

            Log.Info(">> Sienna Logon is online on port " + LogonConfig.get.LoginPort);

            while (true) { Thread.Sleep(Timeout.Infinite); }
        }
    }
}
