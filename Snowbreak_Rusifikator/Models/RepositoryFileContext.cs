using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator.Models
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(List<RepositoryFile>))]
    internal partial class RepositoryFileContext : JsonSerializerContext
    {
    }
}
