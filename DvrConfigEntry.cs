using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class DvrConfigEntry
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class DvrConfigEntryList
    {
        [JsonPropertyName("total")]
        public int Count { get; set; }

        [JsonPropertyName("entries")]
        public List<DvrConfigEntry> Entries { get; set; }
        }
    
}
