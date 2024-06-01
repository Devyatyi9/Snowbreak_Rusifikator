using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Text.Json.Serialization.Metadata;

namespace Snowbreak_Rusifikator.Models
{
    public record class RepositoryFile(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("sha")] string Sha,
        [property: JsonPropertyName("download_url")] Uri DownloadUrl);
}
