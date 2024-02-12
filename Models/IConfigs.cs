using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator
{
    public interface IConfigs
    {
        public class ProgramConfig
        {
            //\game\Game\Content\Paks
            public required string gamePath { get; set; }
            public required string launcherPath { get; set; }
            public required string fileName { get; set; }
            public bool isTester { get; set; }
            public required string sha { get; set; }
        }
        public class JsonGamePreference
        {
            public required string dataPath { get; set; }
            public bool enableSpeedLimit { get; set; }
            public required string lang { get; set; }
            public ushort maxDownloadSpeed { get; set; }
        }
    }
}
