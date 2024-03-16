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
    public class RepositoryFile : DefaultJsonTypeInfoResolver
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("sha")]
        public string Sha { get; set; }
        [JsonPropertyName("download_url ")]
        public Uri DownloadUrl { get; set; }
    }
}
