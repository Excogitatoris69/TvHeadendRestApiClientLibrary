
namespace TvHeadendRestApiClientLibrary
{
    /// <summary>
    /// Contains all neccesarry data for send request to TvHeadendServer.
    /// </summary>
    public class RequestData
    {

        public string ServerUrl { get; set; }
        public string ServerUrlApi { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Command { get; set; }
        public long Starttime { get; set; }
        public long Endtime { get; set; }
        public string ChannelName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Uuid { get; set; }
        public string Language { get; set; }
        public string DvrProfileName { get; set; }
        public string Comment { get; set; }
        public Priority Priority { get; set; }
        public string StreamplayerPath { get; set; }
    }
}
