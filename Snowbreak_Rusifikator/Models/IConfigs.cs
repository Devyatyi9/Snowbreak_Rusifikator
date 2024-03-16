using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator
{
    public interface IConfigs
    {
        public class ProgramConfig : DefaultJsonTypeInfoResolver
        {
            public string gamePath { get; set; }
            public string launcherPath { get; set; }
            public string fileName { get; set; }
            public bool isTester { get; set; }
            public string sha { get; set; }
        }
        public class JsonGamePreference : DefaultJsonTypeInfoResolver
        {
            public string dataPath { get; set; }
            public bool enableSpeedLimit { get; set; }
            public string lang { get; set; }
            public ushort maxDownloadSpeed { get; set; }
        }
    }
}
