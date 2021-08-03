using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class DvrAddEntry
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("stop")]
        public long Stop { get; set; }

        [JsonPropertyName("channelname")]
        public string Channelname { get; set; }

        [JsonPropertyName("pri")]
        public Priority Priority { get; set; }

        [JsonPropertyName("title")]
        public DvrTitle Title { get; set; }

        [JsonPropertyName("subtitle")]
        public DvrTitle Description { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("config_name")]
        public string DvrProfileUuid { get; set; }
    }
    
}
