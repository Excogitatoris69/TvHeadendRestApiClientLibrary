using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvHeadendRestApiClientLibrary
{
    public class EpgFilter
    {
        public EpgFilter()
        {
            this.Channelname = null;
            this.NowMode = false;
            this.Limit = 0;
            this.Start = 0;
            this.DurationMin = -1;
            this.DurationMax = -1;
        }

        public string Channelname { get; set; }

        public bool NowMode { get; set; }

        public int DurationMin { get; set; }

        public int DurationMax { get; set; }

        public int Start { get; set; }

        public int Limit { get; set; }

            
    }
    
}
