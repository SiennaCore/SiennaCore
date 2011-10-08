/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FrameWork;
using Common;

namespace RealmServer
{
    class Program
    {
        static public RealmConfig Config;
        static public RpcClient Client;
        static public RpcServer Server;

        static public AccountMgr Accounts;
        static public CharactersMgr Characters;
        static public WorldMgr World;

        static void Main(string[] args)
        {
            Log.Texte("", "-------------------------------", ConsoleColor.DarkBlue);
            Log.Texte("", "          _____   _____ ", ConsoleColor.Cyan);
            Log.Texte("", "    /\\   |  __ \\ / ____|", ConsoleColor.Cyan);
            Log.Texte("", "   /  \\  | |__) | (___  ", ConsoleColor.Cyan);
            Log.Texte("", "  / /\\ \\ |  ___/ \\___ \\ ", ConsoleColor.Cyan);
            Log.Texte("", " / ____ \\| |     ____) |", ConsoleColor.Cyan);
            Log.Texte("", "/_/    \\_\\_|    |_____/ Rift", ConsoleColor.Cyan);
            Log.Texte("", "http://AllPrivateServer.com", ConsoleColor.DarkCyan);
            Log.Texte("", "-------------------------------", ConsoleColor.DarkBlue);

            // Loading all configs files
            ConfigMgr.LoadConfigs();
            Config = ConfigMgr.GetConfig<RealmConfig>();
            Config.RealmInfo.GenerateName();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel,"Realm"))
                ConsoleMgr.WaitAndExit(2000);

            CharactersMgr.CharactersDB = DBManager.Start(Config.CharactersDB.Total(), ConnectionType.DATABASE_MYSQL, "Characters");
            if (CharactersMgr.CharactersDB == null)
                ConsoleMgr.WaitAndExit(2000);

            WorldMgr.WorldDB = DBManager.Start(Config.WorldDB.Total(), ConnectionType.DATABASE_MYSQL, "World");
            if (WorldMgr.WorldDB == null)
                ConsoleMgr.WaitAndExit(2000);

            PacketProcessor.RegisterDefinitions();

            // Starting Remote Client
            Client = new RpcClient("Realm-" + Config.RealmInfo.RealmId, Config.RpcCharacter.RpcLocalIp, 1);
            if (!Client.Start(Config.RpcCharacter.RpcServerIp, Config.RpcCharacter.RpcServerPort))
                ConsoleMgr.WaitAndExit(2000);


            Server = new RpcServer(Config.RpcMapServer.RpcClientStartingPort, 2);
            if (!Server.Start(Config.RpcMapServer.RpcIp, Config.RpcMapServer.RpcPort))
                ConsoleMgr.WaitAndExit(2000);

            World = Client.GetLocalObject<WorldMgr>();
            Accounts = Client.GetServerObject<AccountMgr>();
            Characters = Client.GetLocalObject<CharactersMgr>();

            // 1 : Loading WorldMgr
            World.Load();

            // 2 : Loading CharactersMgr
            CharactersMgr.Client = Client;
            CharactersMgr.MyRealm = Config.RealmInfo;
            CharactersMgr.MyRealm.RpcInfo = Client.Info;
            Characters.Load();

            // 3 : Loading AccountsMgr
            Accounts.RegisterRealm(Config.RealmInfo, Client.Info);
            
            ConsoleMgr.Start();
        }
    }
}
