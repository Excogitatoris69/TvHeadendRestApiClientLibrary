using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace TvHeadendRestApiClientLibrary
{
    /// <summary>
    /// Provides a library of functions to access the TvHeadend server.
    /// This is free software that I made for myself in my spare time. I offer these freely, without financial intentions. 
    /// Necessary TvHeadend-Api-Version: 19
    /// TvHeadend-Version: 4.2.8
    /// 
    /// Author: Oliver Matle
    /// Date: August, 2021
    /// </summary>
    /// <seealso cref="https://github.com/Excogitatoris69/TvHeadendRestApiClientLibrary"/>
    /// <seealso cref="https://tvheadend.org/"/>
    /// <seealso cref="https://www.tvbrowser.org/"/>
    public class TvHeadendLibrary
    {
        private HttpClient tvHeadendHttpclient = null;
        private readonly string defaultLanguage = "und"; // tvheadend language -> "und" = "undetermined"
        private readonly string defaultcomment = ""; 
        public static readonly string RELEASESTRING = "1.0.0 , August 2021";

        /// <summary>
        /// Returns a list of all channels defined in TvHeadend.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ChannelEntryList GetChannellist(RequestData requestData)
        {
            if (requestData == null)
                throw new NullReferenceException("RequestData is null.");
            requestData.ServerUrlApi = "/api/channel/list";
            var resultString = CallServer(requestData);

            ChannelEntryList aChannelEntryList = (ChannelEntryList)JsonSerializer.Deserialize<ChannelEntryList>(resultString);
            if (aChannelEntryList == null || aChannelEntryList.Entries.Count == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_NO_DATA_FOUND + ": List of channel is empty. ");
            }
            return aChannelEntryList;
        }

        /// <summary>
        /// Returns a list of all languages defined in TvHeadend.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public LanguageEntryList GetLanguagelist(RequestData requestData)
        {

            if (requestData == null)
                throw new NullReferenceException("RequestData is null.");
            requestData.ServerUrlApi = "/api/language/list";
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            LanguageEntryList aLanguageEntryList = (LanguageEntryList)JsonSerializer.Deserialize<LanguageEntryList>(resultString);
            if(aLanguageEntryList == null || aLanguageEntryList.Entries.Count == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_NO_DATA_FOUND + ": List of languages is empty. ");
            }
            return aLanguageEntryList;
        }
            
        /// <summary>
        /// Returns versioninfos about TvHeadend and Rest-API.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public Serverinfo GetServerinfo(RequestData requestData) 
        {

            if (requestData == null)
                throw new NullReferenceException(Messages.MESSAGE_INVALID_REQUESTDATA + ". RequestData is null.");
            requestData.ServerUrlApi = "/api/serverinfo";
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            Serverinfo aServerinfo = (Serverinfo)JsonSerializer.Deserialize<Serverinfo>(resultString);
            if (aServerinfo == null)
            {
                throw new TvHeadendException(Messages.MESSAGE_NO_DATA_FOUND + ": Serverinfo is empty. ");
            }
            return aServerinfo;
        }

        /// <summary>
        /// Returns a list of the currently-scheduled recordings.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public DvrUpcomingEntryList GetDvrUpcominglist(RequestData requestData)
        {
            if (requestData == null)
                throw new NullReferenceException("RequestData is null.");
            requestData.ServerUrlApi = "/api/dvr/entry/grid_upcoming";
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            DvrUpcomingEntryList aDvrUpcomingEntryList = (DvrUpcomingEntryList)JsonSerializer.Deserialize<DvrUpcomingEntryList>(resultString);
            return aDvrUpcomingEntryList;
        }

        /// <summary>
        /// Removes a completed recording from storage.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public TvHeadendResponseData RemoveDvrEntry(RequestData requestData)
        {
            DvrUpcomingEntryList list = null;
            string requestUuid = null;
            TvHeadendResponseData aTvHeadendResponseData = new TvHeadendResponseData();
            aTvHeadendResponseData.Uuid = null;
            list = GetDvrUpcominglist(requestData);
                
            if (list.Entries != null)
            {
                foreach (DvrUpcomingEntry entry in list.Entries)
                {
                    //search for either starttime and channelname or for uuid
                    if(requestData.Uuid != null)
                    {
                        if (entry.Owner == requestData.UserName && entry.Uuid == requestData.Uuid)
                        {
                            requestUuid = entry.Uuid;
                            break;
                        }
                    } else if(requestData.Starttime != 0 && requestData.ChannelName != null) {
                        if (!CheckChannelname(requestData, requestData.ChannelName))
                        {
                            throw new TvHeadendException(Messages.MESSAGE_INVALID_CHANNELNAME);
                        }
                        if (entry.Owner == requestData.UserName && entry.Start == requestData.Starttime && entry.Channelname == requestData.ChannelName)
                        {
                            requestUuid = entry.Uuid;
                            break;
                        }
                    }
                    else
                    {
                        throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Invalid Channel, starttime or uuid.");
                    }
                }
                if (requestUuid == null)
                {
                    throw new TvHeadendException(Messages.MESSAGE_UNKNOWN_DVR_ENTRY);
                }
            }
            requestData.ServerUrlApi = "/api/dvr/entry/cancel?uuid=" + requestUuid;
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            aTvHeadendResponseData.Uuid = requestUuid;
            return aTvHeadendResponseData;
        }

        /// <summary>
        /// Creates a new epg-derived timer.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public TvHeadendResponseData AddDvrEntry(RequestData requestData)
        {
            string dvrConfigUuid = null;
            TvHeadendResponseData aTvHeadendResponseData = null;

            if (requestData == null)
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". RequestData is null.");

            if (requestData.DvrProfileName != null)
            {
                // get uuid of DVR-Profilename, if it is not null
                bool validDvrConfigname = CheckDvrConfigname(requestData, requestData.DvrProfileName, out dvrConfigUuid);
                if(!validDvrConfigname)
                    throw new TvHeadendException(Messages.MESSAGE_INVALID_DVR_CONFIGNAME + ":" + requestData.DvrProfileName);
            }
                            
            if (!CheckChannelname(requestData, requestData.ChannelName))
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_CHANNELNAME+":"+requestData.ChannelName);
            }
            if (requestData.Language != null)
            {
                if (!CheckLanguage(requestData, requestData.Language))
                {
                    throw new TvHeadendException(Messages.MESSAGE_INVALID_LANGUAGE + ":" + requestData.Language);
                }
            }
            if (requestData.Title == null || requestData.Title.Length == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Title not set or empty.");
            }
            if (requestData.Description == null || requestData.Description.Length == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Description not set or empty.");
            }
            if (requestData.Starttime == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Starttime not set.");
            }
            if (requestData.Endtime == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Endtime not set.");
            }
            if(requestData.Starttime >= requestData.Endtime)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_TIME + ". Starttime must be lower than Endtime.");
            }
            long  unixTimestampNow = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (requestData.Endtime < unixTimestampNow)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". The end time must not be in the past.");
            }

            // build Json-Object
            DvrAddEntry dvrAddEntry = new DvrAddEntry();
            dvrAddEntry.Enabled = true;
            dvrAddEntry.Start = requestData.Starttime;
            dvrAddEntry.Stop = requestData.Endtime;
            dvrAddEntry.Channelname = requestData.ChannelName;
            if (requestData.Priority == Priority.Unknown)
                dvrAddEntry.Priority = Priority.Normal;
            else
                dvrAddEntry.Priority = requestData.Priority;
            dvrAddEntry.DvrProfileUuid = dvrConfigUuid;


            DvrTitle aTitle = new DvrTitle();
            DvrTitle aSubTitle = new DvrTitle();
            aTitle.Language = new Dictionary<string, object>();
            aSubTitle.Language = new Dictionary<string, object>();
            if (requestData.Language == null)
            {
                aTitle.Language.Add(defaultLanguage, requestData.Title);
                aSubTitle.Language.Add(defaultLanguage, requestData.Description);
            }
            else{
                aTitle.Language.Add(requestData.Language, requestData.Title);
                aSubTitle.Language.Add(requestData.Language, requestData.Description);
            }
            dvrAddEntry.Title = aTitle;
            dvrAddEntry.Description = aSubTitle;
                
            if (requestData.Comment == null)
            {
                dvrAddEntry.Comment = defaultcomment;
            }
            else
            {
                dvrAddEntry.Comment = requestData.Comment;
            }

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };
            string jsonDvrRequestString = JsonSerializer.Serialize(dvrAddEntry, options);
            //Console.WriteLine(jsonDvrRequestString);
            string encodedUrl = HttpUtility.UrlEncode(jsonDvrRequestString);
            requestData.ServerUrlApi = "/api/dvr/entry/create?conf=" + encodedUrl;
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            aTvHeadendResponseData = (TvHeadendResponseData)JsonSerializer.Deserialize<TvHeadendResponseData>(resultString);
            if (aTvHeadendResponseData.Uuid == null || aTvHeadendResponseData.Uuid.Length == 0)
            {
                throw new TvHeadendException(Messages.MESSAGE_DVR_CREATE_ENTRY_UNSUCCESSFUL);
            }
            return aTvHeadendResponseData;
        }


        public DvrConfigEntryList GetDvrConfiglist(RequestData requestData)
        {
            if (requestData == null)
                throw new NullReferenceException("RequestData is null.");
            requestData.ServerUrlApi = "/api/dvr/config/grid";
            var resultString = CallServer(requestData);
            //Console.WriteLine(resultString);
            DvrConfigEntryList aDvrConfigEntryList = (DvrConfigEntryList)JsonSerializer.Deserialize<DvrConfigEntryList>(resultString);
            return aDvrConfigEntryList;
        }

        /// <summary>
        /// Query the EPG and optionally apply filters.
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="epgFilter"></param>
        /// <returns></returns>
        public EpgEntryList GetEpgEntryList(RequestData requestData, EpgFilter epgFilter)
        {
            if (requestData == null)
                throw new NullReferenceException("RequestData is null.");
            StringBuilder filterString = null;
            //set filter
            if(epgFilter == null)
            {
                requestData.ServerUrlApi = "/api/epg/events/grid";
            }else
            {
                filterString = new StringBuilder("/api/epg/events/grid?");
                if (epgFilter.Start > 0) //default is 0
                    filterString.Append("start=").Append(epgFilter.Start);
                if (epgFilter.Limit > 0) //default is 50
                    filterString.Append(",limit=").Append(epgFilter.Limit);
                if (epgFilter.Channelname != null) 
                    filterString.Append(",channel=").Append(epgFilter.Channelname);
                if (epgFilter.NowMode) 
                    filterString.Append(",mode=now");
                if (epgFilter.DurationMin >= 0)
                    filterString.Append(",durationMin=").Append(epgFilter.DurationMin);
                if (epgFilter.DurationMax >= 0)
                    filterString.Append(",durationMax=").Append(epgFilter.DurationMax);

                filterString.Replace("?,","?");
                requestData.ServerUrlApi = filterString.ToString();
            }
            //?start=0&limit=200&channel=ProSieben,ZDF&durationMin=1&durationMax=1000

            var resultString = CallServer(requestData);
            Console.WriteLine(filterString.ToString());
            EpgEntryList aEpgEntryList = (EpgEntryList)JsonSerializer.Deserialize<EpgEntryList>(resultString);
            return aEpgEntryList;
        }


        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private string CallServer(RequestData requestData)
        {
            string result = null;
            HttpResponseMessage resonse = null;

            if (tvHeadendHttpclient == null)
            {
                tvHeadendHttpclient = new HttpClient();
                var authToken = Encoding.ASCII.GetBytes($"{requestData.UserName}:{requestData.Password}");
                tvHeadendHttpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            }
            try
            {
                resonse = tvHeadendHttpclient.GetAsync(requestData.ServerUrl + requestData.ServerUrlApi).Result;
            } 
            catch(System.AggregateException e)
            {
                throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ". Possibly wrong serverurl or wrong port. Error:" + e.Message);
            }
            if (resonse.IsSuccessStatusCode)
            {
                var responseContent = resonse.Content;
                // by calling .Result you are synchronously reading the result
                result = responseContent.ReadAsStringAsync().Result;
                //Console.WriteLine("Result:"+ result);
            }
            else
            {
                switch (resonse.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ": Wrong Username or password.");
                    default:
                        throw new TvHeadendException(Messages.MESSAGE_INVALID_REQUESTDATA + ": " + resonse.StatusCode);
                }
            }
            return result;
        }

        /// <summary>
        /// Check for valid DVR-Configurationname
        /// </summary>
        /// <param name="requestdata"></param>
        /// <param name="configname"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        private bool CheckDvrConfigname(RequestData requestdata, string configname, out string uuid)
        {
            bool result = true;
            uuid = null;
            DvrConfigEntryList list = GetDvrConfiglist(requestdata);
            if(list.Entries != null && list.Entries.Count > 0)
            {
                foreach( DvrConfigEntry entry in list.Entries)
                {
                    if(entry.Name == configname)
                    {
                        uuid = entry.Uuid;
                        break;
                    }
                }
            }
            else
            {
                result = false; // configname do not exist, if empty list.
            }
            return result;
        }

        /// <summary>
        /// Check for valid channelname.
        /// </summary>
        /// <param name="requestdata"></param>
        /// <param name="channelname"></param>
        /// <returns></returns>
        private bool CheckChannelname(RequestData requestdata, string channelname)
        {
            bool result = false;
            ChannelEntryList list = GetChannellist(requestdata);
            if (channelname != null && channelname.Length > 0 && list.Entries != null)
            {
                foreach (ChannelEntry entry in list.Entries)
                {
                    if (entry.Name == channelname)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Check for valid language.
        /// </summary>
        /// <param name="requestdata"></param>
        /// <param name="language"></param>
        /// <returns>True, if valid.</returns>
        private bool CheckLanguage(RequestData requestdata, string language)
        {
            bool result = false;
            LanguageEntryList list = GetLanguagelist(requestdata);
            if (language != null && language.Length >0 && list.Entries != null)
            {
                foreach (LanguageEntry entry in list.Entries)
                {
                    if (entry.Shortname == language)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

       
    }

}
