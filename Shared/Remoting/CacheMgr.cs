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

        public WorldTextEntry GetTextCache(long Entry)
        {
            TextEntry entry = CharacterDB.SelectObject<TextEntry>("Id=" + Entry + "");

            WorldTextEntry tentry = new WorldTextEntry();
            tentry.Entry = entry.Id;
            tentry.Text = entry.Loc_1;

            return tentry;
        }

        public WorldZoneInfo GetZoneInfoCache(string Zone)
        {
            ZoneEntry entry = CharacterDB.SelectObject<ZoneEntry>("Name='" + CharacterDB.Escape(Zone) + "'");

            WorldZoneInfo ZoneInfo = new WorldZoneInfo();
            ZoneInfo.ZoneFileName = entry.Name;
            ZoneInfo.Description = GetTextCache(entry.Desc);
            ZoneInfo.DisplayName = GetTextCache(entry.DisplayName);

            return ZoneInfo;
        }

        public List<CacheEntry> GetBinCache(long Opcode, bool IsOnRead)
        {
            CacheEntry[] entries = (CacheEntry[])CharacterDB.SelectObjects<CacheEntry>("Opcode=" + Opcode + " AND OnRead=" + (IsOnRead == true ? 1 : 0));
            return new List<CacheEntry>(entries);
        }
    }
}
