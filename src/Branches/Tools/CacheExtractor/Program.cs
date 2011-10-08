using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FrameWork;
using Common;

namespace CacheExtractor
{
    class Program
    {
        static public ExtractorConfig Config;
        static public MySQLObjectDatabase WorldDB;

        [STAThread]
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
            Config = ConfigMgr.GetConfig<ExtractorConfig>();

            // Loading log level from file
            if (!Log.InitLog(Config.LogLevel, "Cache"))
                ConsoleMgr.WaitAndExit(2000);

            WorldDB = DBManager.Start(Config.WorldDB.Total(), ConnectionType.DATABASE_MYSQL, "World");
            if (WorldDB == null)
                ConsoleMgr.WaitAndExit(2000);

            PacketProcessor.RegisterDefinitions();

            int Count = 0;

            foreach (string FileDir in Directory.EnumerateFiles(Config.CacheFolder))
            {
                Log.Success("Extractor", "Opening Cache : " + FileDir);

                FileStream Str = File.Open(FileDir, FileMode.Open);

                byte[] Buff = new byte[Str.Length];
                Str.Read(Buff,0,Buff.Length);

                PacketInStream Pack = new PacketInStream(Buff, Buff.Length);
                ISerializablePacket Packet = PacketProcessor.ReadPacket(ref Pack);


                if (Packet != null && Packet is CacheUpdate)
                {
                    CacheUpdate Cache = Packet as CacheUpdate;

                    CacheTemplate Template = Cache.CacheDatas.Find(info => info.Opcode == (long)Opcodes.CacheTemplate) as CacheTemplate;
                    CacheData Data = Cache.CacheDatas.Find(info => info.Opcode == (long)Opcodes.CacheData) as CacheData;

                    if (Template != null)
                    {
                        Template.CacheID = Cache.CacheID;
                        Template.CacheType = Cache.CacheType;
                        WorldDB.AddObject(Template);
                    }

                    if (Data != null)
                    {
                        Data.CacheID = Cache.CacheID;
                        Data.CacheType = Cache.CacheType;
                        WorldDB.AddObject(Data);
                    }
                    ++Count;
                }

                Str.Dispose();
            }

            Log.Success("Extractor", "" + Count + " Caches extracted");

            ConsoleMgr.Start();
        }
    }
}
