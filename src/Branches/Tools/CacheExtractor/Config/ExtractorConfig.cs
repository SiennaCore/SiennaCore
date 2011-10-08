using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace CacheExtractor
{
    [aConfigAttributes("Configs/CacheExtractor.xml")]
    public class ExtractorConfig : aConfig
    {
        public string CacheFolder = "CacheData";
        public DatabaseInfo WorldDB = new DatabaseInfo();
        public LogInfo LogLevel = new LogInfo();
    }
}
