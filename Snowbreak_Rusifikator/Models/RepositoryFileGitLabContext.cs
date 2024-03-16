using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Snowbreak_Rusifikator.Models
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(List<RepositoryFileGitLab>))]
    internal partial class RepositoryFileGitLabContext : JsonSerializerContext
    {
    }
}
