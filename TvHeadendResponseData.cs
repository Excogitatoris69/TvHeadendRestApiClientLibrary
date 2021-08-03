using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
        public class TvHeadendResponseData
        {
            [JsonPropertyName("uuid")]
            public string Uuid { get; set; }
        }
    
}
