using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class Serverinfo
    {
        [JsonPropertyName("api_version")]
        public int VersionApi { get; set; }

        [JsonPropertyName("sw_version")]
        public string VersionSoftware { get; set; }

    }
    
}
