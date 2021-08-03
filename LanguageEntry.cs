using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class LanguageEntry
    {
        [JsonPropertyName("key")]
        public string Shortname { get; set; }

        [JsonPropertyName("val")]
        public string Longname { get; set; }

    }
    public class LanguageEntryList
    {
        [JsonPropertyName("entries")]
        public List<LanguageEntry> Entries { get; set; }
    }
    
}
