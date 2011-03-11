using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Principal;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using Shared;
using Shared.NetWork;
using Shared.Database;
using System.Security.Cryptography;

namespace CharacterServer
{
    class Program
    {
        static public AccountMgr AcctMgr = null;
        static public CharacterMgr CharMgr = null;
        static public CharacterConfig Config = null;

        static void Main(string[] args)
        {
            Log.Texte("", "-------------------------------", ConsoleColor.DarkBlue);
            Log.Texte("", ",---.o", ConsoleColor.Cyan);
            Log.Texte("", "`---..,---.,---.,---.,---.", ConsoleColor.Cyan);
            Log.Texte("", "    |||---'|   ||   |,---|", ConsoleColor.Cyan);
            Log.Texte("", "`---'``---'`   '`   '`---^ Core", ConsoleColor.Cyan);
            Log.Texte("", "http://siennacore.com", ConsoleColor.Blue);
            Log.Texte("", "-------------------------------", ConsoleColor.DarkBlue);

            // Loading log level from file
            if (!Log.InitLog("Configs/Characters.log", "Characters"))
                WaitAndExit();

            // Loading all configs files
            ConfigMgr.LoadConfigs();
            Config = ConfigMgr.GetConfig<CharacterConfig>();

            // Starting Remoting Server
            if (!RpcServer.InitRpcServer("Character", Config.RpcKey, Config.RpcPort))
                WaitAndExit();

            // Creating Remote objects
            AcctMgr = new AccountMgr();
            AccountMgr.AccountDB = DBManager.Start(Config.AccountsDB.Total(), ConnectionType.DATABASE_MYSQL, "Accounts");
            if (AccountMgr.AccountDB == null)
                WaitAndExit();

            CharMgr = new CharacterMgr();
            CharacterMgr.CharacterDB = DBManager.Start(Config.CharactersDB.Total(), ConnectionType.DATABASE_MYSQL, "Characters");
            if (CharacterMgr.CharacterDB == null)
                WaitAndExit();

            CharMgr.LoadRealms();
            CharMgr.LoadRandomNames();

            // Listening Client
            if (!TCPManager.Listen<RiftServer>(Config.CharacterServerPort, "Character"))
                WaitAndExit();

            ConsoleMgr.Start();
        }

        static public void WaitAndExit()
        {
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
