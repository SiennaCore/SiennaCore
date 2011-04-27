using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared;
using Shared.Database;

namespace Shared
{
    [RpcAttributes(new string[] { "CharacterServer", "WorldServer" })]
    public class CacheMgr : ARpc
    {
        static public ObjectDatabase CharacterDB = null;
        static public CacheMgr Instance = null;

        public CacheMgr()
        {
            if (Instance == null)
                Instance = this;
        }

        public WorldText_Info GetTextCache(long Entry)
        {
            Text_Info entry = CharacterDB.SelectObject<Text_Info>("Id=" + Entry + "");

            WorldText_Info tentry = new WorldText_Info();
            tentry.Entry = entry.Id;
            tentry.Text = entry.Loc_1;

            return tentry;
        }

        public WorldZoneInfo GetZoneInfoCache(string Zone)
        {
            Zone_Info entry = CharacterDB.SelectObject<Zone_Info>("Name='" + CharacterDB.Escape(Zone) + "'");

            WorldZoneInfo ZoneInfo = new WorldZoneInfo();
            ZoneInfo.ZoneFileName = entry.Name;
            ZoneInfo.Description = GetTextCache(entry.Desc);
            ZoneInfo.DisplayName = GetTextCache(entry.DisplayName);

            return ZoneInfo;
        }

        public List<Cache_Info> GetBinCache(long Opcode, bool IsOnRead)
        {
            Cache_Info[] entries = (Cache_Info[])CharacterDB.SelectObjects<Cache_Info>("Opcode=" + Opcode + " AND OnRead=" + (IsOnRead == true ? 1 : 0));
            return new List<Cache_Info>(entries);
        }
    }
}
