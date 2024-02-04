using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Snowbreak_Rusifikator
{
    public record class RepositoryFile(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("sha")] string Sha,
        [property: JsonPropertyName("download_url")] Uri DownloadUrl);
}
