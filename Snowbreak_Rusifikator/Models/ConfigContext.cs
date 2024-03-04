using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Snowbreak_Rusifikator.IConfigs;

namespace Snowbreak_Rusifikator.Models
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ProgramConfig))]
    [JsonSerializable(typeof(JsonGamePreference))]
    internal partial class ConfigContext : JsonSerializerContext
    {
    }
}
