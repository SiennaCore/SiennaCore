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


namespace MapServer
{
    class Program
    {
        static public MapConfig Config;
        static public RpcClient Client;

        static public Realm Rm = null;

        static public AccountMgr Accounts
        {
            get
            {
                return Characters.GetAccounts();
            }
        }
        static public CharactersMgr Characters
        {
            get
            {
                return Rm.GetObject<CharactersMgr>();
            }
        }
        static public WorldMgr World
        {
            get
            {
                return Rm.GetObject<WorldMgr>();
            }
        }
        static public MapMgr Maps
        {
            get
            {
                return Client.GetLocalObject<MapMgr>();
            }
        }

        static public byte[] BuildPlayer;

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
            Config = ConfigMgr.GetConfig<MapConfig>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "Map"))
                ConsoleMgr.WaitAndExit(2000);


            FileStream Str = File.Open("player.cache", FileMode.Open);
            BuildPlayer = new byte[Str.Length];
            Str.Read(BuildPlayer, 0, BuildPlayer.Length);

            /*// Listening Client
            if (!TCPManager.Listen<RiftServer>(Config.ServerInfo.MapPort, "CharacterServer"))
                ConsoleMgr.WaitAndExit(2000);

            PacketInStream Entity = new PacketInStream(BuildPlayer,BuildPlayer.Length);
            WorldEntityUpdate Update = PacketProcessor.ReadPacket(ref Entity) as WorldEntityUpdate;
            Log.Info("Entity", "GUID = " + Update.GUID + " List Lengh = " + Update.Field1.Count);
            Console.ReadKey();
            Environment.Exit(0);*/

            // Starting Remote Client
            Client = new RpcClient("Map-" + Config.ServerInfo.MapAdress, Config.ClientInfo.RpcLocalIp, 2);
            if (!Client.Start(Config.ClientInfo.RpcServerIp, Config.ClientInfo.RpcServerPort))
                ConsoleMgr.WaitAndExit(2000);

            // Listening Client
            if (!TCPManager.Listen<RiftServer>(Config.ServerInfo.MapPort, "CharacterServer"))
                ConsoleMgr.WaitAndExit(2000);

            MapMgr.Client = Client;
            MapMgr.MapInfo = Config.ServerInfo;
            MapMgr.MapInfo.RpcInfo = Client.Info;
            Rm = Client.GetServerObject<CharactersMgr>().GetRealm();

            Log.Success("Realm","Connected to : " + Rm.Name);

            World.RegisterMaps(Config.ServerInfo, Client.Info);

            ConsoleMgr.Start();
        }
    }
}
