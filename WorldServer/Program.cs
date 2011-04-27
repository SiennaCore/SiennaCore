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

namespace WorldServer
{
    class Program
    {
        static public WorldConfig Config = null;

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
            if (!Log.InitLog("Configs/World.log", "World"))
                WaitAndExit();

            // Loading all configs files
            ConfigMgr.LoadConfigs();
            Config = ConfigMgr.GetConfig<WorldConfig>();

            // Starting Remoting Server
            if (!RpcClient.InitRpcClient("WorldServer", Config.RpcKey,Config.RpcIp, Config.RpcPort))
                WaitAndExit();

            // Creating Remote objects
            AccountMgr.Instance = new AccountMgr();
            CharacterMgr.Instance = new CharacterMgr();
            CacheMgr.Instance = new CacheMgr();

            // Listening Client
            if (!TCPManager.Listen<RiftServer>(Config.WorldServerPort, "WorldServer"))
                WaitAndExit();

            Realm Rm = CharacterMgr.Instance.RegisterRealm(Config.RealmId, Config.WorldServerIP, Config.WorldServerPort, RpcClient.GetRpcClientId("WorldServer"));
            if (Rm == null)
            {
                Log.Error("WorldServer", "Invalid Realm : " + Config.RealmId);
                WaitAndExit();
            }

            ConsoleMgr.Start();
        }

        static public void WaitAndExit()
        {
            System.Threading.Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
