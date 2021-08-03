using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class EpgEntry
    {

        [JsonPropertyName("channelName")]
        public string Channelname { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("stop")]
        public long Stop { get; set; }

        [JsonPropertyName("dvrUuid")]
        public string DvrUuid { get; set; }

        [JsonPropertyName("dvrState")]
        public string DvrState { get; set; }

    }

    public class EpgEntryList
    {
        [JsonPropertyName("totalCount")]
        public int Count { get; set; }

        [JsonPropertyName("entries")]
        public List<EpgEntry> Entries { get; set; }
    }

    
}
