using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    
    /// <summary>
    /// Contains channelname and uuid
    /// </summary>
    public class ChannelEntry
    {
        [JsonPropertyName("key")]
        public string Uuid { get; set; }

        [JsonPropertyName("val")]
        public string Name { get; set; }

    }

    /// <summary>
    /// Contains list of channelentries.
    /// TvHeadend-Server send Data in this json format:
    ///        <code>
    ///        {
    ///        "entries":
    ///         [
    ///            {"key":"d8518d80f7633510846bad57f56eea59","val":"RTLplus"},
    ///            {"key":"47db1d02b583e93e1a64e3b4feb3001e","val":"3sat"},
    ///            {"key":"72c87a034b59d91d005c08cb5e342c92","val":"NDR FS MV"}
    ///         ]
    ///        } 
    /// </code>
    /// </summary>
    public class ChannelEntryList
    {
        [JsonPropertyName("entries")]
        public List<ChannelEntry> Entries { get; set; }
    }
    
}
