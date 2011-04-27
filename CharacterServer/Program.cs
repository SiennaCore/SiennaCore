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
            if (!RpcServer.InitRpcServer("CharacterServer", Config.RpcKey, Config.RpcPort))
                WaitAndExit();

            // Creating Remote objects
            new AccountMgr();
            AccountMgr.AccountDB = DBManager.Start(Config.AccountsDB.Total(), ConnectionType.DATABASE_MYSQL, "Accounts");
            if (AccountMgr.AccountDB == null)
                WaitAndExit();

            new CharacterMgr();
            CharacterMgr.CharacterDB = DBManager.Start(Config.CharactersDB.Total(), ConnectionType.DATABASE_MYSQL, "Characters");
            if (CharacterMgr.CharacterDB == null)
                WaitAndExit();

            new CacheMgr();
            CacheMgr.CharacterDB = DBManager.Start(Config.CharactersDB.Total(), ConnectionType.DATABASE_MYSQL, "Characters");
            if (CacheMgr.CharacterDB == null)
                WaitAndExit();

            CharacterMgr.Instance.LoadRealms();
            CharacterMgr.Instance.LoadCreation_Names();

            // Listening Client
            if (!TCPManager.Listen<RiftServer>(Config.CharacterServerPort, "CharacterServer"))
                WaitAndExit();

            /*long Data = 0x17;
            int FieldType;
            int Index;
            PacketInStream.Decode2Parameters(Data, out FieldType, out Index);

            Log.Success("Test", "Type=" + FieldType + ",Index=" + Index);*/

            ConsoleMgr.Start();
        }

        static public void WaitAndExit()
        {
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
