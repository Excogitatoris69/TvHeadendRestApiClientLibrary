using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class DvrTitle
    {
        [JsonExtensionData]
        public Dictionary<string, object> Language { get; set; }

        /// <summary>
        /// Get the first occurrence of the film regardless of the language.
        /// </summary>
        /// <returns></returns>
        public string GetFirstTitle()
        {
            string result = null;
            object obj = null;
            if (Language != null && Language.Count > 0)
            {
                foreach(string key in Language.Keys)
                {
                    Language.TryGetValue(key, out obj);
                    if(obj != null)
                    {
                        result = ((JsonElement)obj).GetString();
                        break;
                    }
                }
            }
            return result;
        }

    }
    
}
