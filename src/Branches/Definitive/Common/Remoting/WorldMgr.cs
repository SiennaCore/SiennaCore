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

using FrameWork;

namespace Common
{
    [Rpc(false, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 1)]
    public class WorldMgr : RpcObject
    {
        static public MySQLObjectDatabase WorldDB;

        #region Database

        static public Dictionary<uint, CacheData> Datas;
        static public Dictionary<uint, CacheTemplate> Templates;
        static public Dictionary<long, TextInfo> TextInfos;

        public void Load()
        {
            Log.Success("WorldMgr", "Loading World Database");

            int Start = Environment.TickCount;

            // Here all Load functions
            LoadCache();

            int End = Environment.TickCount;

            Log.Success("WorldMgr", "World loaded in : " + (End - Start) + "ms");
        }

        public TextInfo GetText(long ID)
        {
            TextInfo Info;
            TextInfos.TryGetValue(ID, out Info);
            return Info;
        }

        public CacheData GetData(uint CacheID)
        {
            CacheData Data;
            Datas.TryGetValue(CacheID, out Data);
            return Data;
        }

        public CacheTemplate GetTemplate(uint CacheID)
        {
            CacheTemplate Template;
            Templates.TryGetValue(CacheID,out Template);
            return Template;
        }

        public CacheTemplate[] GetTemplates()
        {
            return Templates.Values.ToArray();
        }

        public CacheData[] GetDatas()
        {
            return Datas.Values.ToArray();
        }

        private void LoadCache()
        {
            bool Debug = Log.Config.Info.Debug;
            bool Error = Log.Config.Info.Error;

            Log.Config.Info.Error = false;
            Log.Config.Info.Debug = false;

            Datas = new Dictionary<uint, CacheData>();
            Templates = new Dictionary<uint, CacheTemplate>();
            TextInfos = new Dictionary<long, TextInfo>();

            CacheData[] Dts = WorldDB.SelectAllObjects<CacheData>().ToArray();
            CacheTemplate[] Cte = WorldDB.SelectAllObjects<CacheTemplate>().ToArray();
            TextInfo[] Tis = WorldDB.SelectAllObjects<TextInfo>().ToArray();

            foreach (TextInfo Txt in Tis)
                TextInfos.Add(Txt.ID, Txt);

            foreach (CacheData Data in Dts)
            {
                Data.Field7 = GetText(Data.TextID_1);
                Data.Field8 = GetText(Data.TextID_2);
                Datas.Add(Data.CacheID, Data);
            }

            foreach (CacheTemplate Tm in Cte)
            {
                Tm.Field40 = GetText(Tm.TextID);
                Templates.Add(Tm.CacheID, Tm);
            }

            Log.Config.Info.Error = Error;
            Log.Config.Info.Debug = Debug;

            Log.Success("LoadCache", "Loaded : " + Datas.Count + Templates.Count + " Caches");
        }

        static public CacheUpdate BuildCache(uint CacheID, long CacheType, ISerializablePacket Packet)
        {
            CacheUpdate Data = new CacheUpdate();
            Data.CacheType = CacheType;
            Data.CacheID = CacheID;
            Data.CacheDatas = new List<ISerializablePacket>() { Packet };
            return Data;
        }

        #endregion

        #region Maps

        public List<MapServerInfo> MapsInfo = new List<MapServerInfo>();

        public MapServerInfo GetMapInfo(string Address)
        {
            return MapsInfo.Find(info => info.MapAdress == Address);
        }

        public MapServerInfo GetMapInfo()
        {
            int MinPlayers = int.MaxValue;
            MapServerInfo MapInfo=null;

            foreach (MapServerInfo Info in MapsInfo)
                if (Info.PlayerCount < MinPlayers)
                {
                    MapInfo = Info;
                    MinPlayers = Info.PlayerCount;
                }

            return MapInfo;
        }

        public void RegisterMaps(MapServerInfo MapInfo, RpcClientInfo RpcInfo)
        {
            MapServerInfo Info = GetMapInfo(MapInfo.MapAdress);

            if (Info == null)
                MapsInfo.Add(MapInfo);
            else
                Info.RpcInfo = RpcInfo;

            MapInfo.RpcInfo = RpcInfo;
            Log.Success("MapMgr", "Map online : " + MapInfo.MapAdress);
        }

        public override void OnClientDisconnected(RpcClientInfo Info)
        {
            foreach(MapServerInfo MapInfo in MapsInfo.ToArray())
                if (MapInfo.RpcInfo.RpcID == Info.RpcID)
                {
                    Log.Error("MapMgr", "MapServer disconnected : " + MapInfo.MapAdress);
                    MapsInfo.Remove(MapInfo);
                }
        }

        #endregion
    }
}
