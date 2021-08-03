using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class DvrUpcomingEntry
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("channelname")]
        public string Channelname { get; set; }

        [JsonPropertyName("title")]
        public DvrTitle Title { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("stop")]
        public long Stop { get; set; }

        [JsonPropertyName("pri")]
        public int Priority { get; set; }

    }

    public class DvrUpcomingEntryList
    {
        [JsonPropertyName("total")]
        public int Count { get; set; }

        [JsonPropertyName("entries")]
        public List<DvrUpcomingEntry> Entries { get; set; }
    }

    
}
