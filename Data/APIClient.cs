using LeagueOfLegendsStats.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsStats.Data
{
    class APIClient
    {
        private string baseURL;

        public string APIKey {get;set;}

        private string _Region;
        public string Region
        {
            get { return _Region; }
            //Update URL when region is changed
            set { _Region = value; baseURL = "https://" + _Region + ".api.riotgames.com/lol/"; }
        }

        public APIClient()
        {
            //Default server
            baseURL = "https://oce1.api.riotgames.com/lol/";
        }

        public async Task<List<Champion>> GetChampions()
        {
            string apiURL = baseURL + "static-data/v3/champions?locale=en_US&dataById=true";

            using (var Client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                Client.DefaultRequestHeaders.Add("Connection", "close");
                try
                {
                    var champList= new List<Champion>();
                    string repURL = apiURL + "&api_key=" + APIKey;

                    string result = "" ;
#if DEBUG_STATIC
                    result = File.OpenText(@"Data\ChampionList.txt").ReadToEnd();
#else
                    var response = Client.GetAsync(repURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                         result = await response.Content.ReadAsStringAsync();
                    }
#endif
                    if (result.Length > 0)
                    {
                        JObject championSearch = JObject.Parse(result);
                        IList<JToken> results = championSearch["data"].Children().ToList();
                        foreach (var child in results)
                        {
                            //The children are retured by id which then contains the champion properties
                            //To ignore the id the child list is selected (always contain a single object hence .First) which is then converted to our class
                            var obj = child.Children().First().ToObject<Champion>();
                            champList.Add(obj);
                        }


                        return champList;
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<Summoner> GetSummoner(string summonerName)
        {
            string apiURL = baseURL + "summoner/v3/summoners/by-name/" + summonerName;
            using (var Client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                Client.DefaultRequestHeaders.Add("Connection", "close");
                try
                {
                    string repURL = apiURL + "?api_key=" + APIKey;
                    string result = "";
#if DEBUG_STATIC
                    result = File.OpenText(@"Data\Summoner.txt").ReadToEnd();
#else
                    var response = Client.GetAsync(repURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                         result = await response.Content.ReadAsStringAsync();
                    }
#endif
                    if (result.Length > 0)
                    {
                        var rootResult = JsonConvert.DeserializeObject<Summoner>(result);
                        return rootResult;
                    }
                    else
                    {
                        return null;
                    }
          
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        internal async Task<MatchData> GetMatch(int gameID)
        {
            string apiURL = baseURL + "match/v3/matches/" + gameID;

            using (var Client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                try
                {
                    string repURL = apiURL + "?api_key=" + APIKey;
                    string result = "";
#if DEBUG_STATIC
                    result = File.OpenText(@"Data\Match.txt").ReadToEnd();
#else
                    var response = Client.GetAsync(repURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                         result = await response.Content.ReadAsStringAsync();
                    }
#endif
                    if (result.Length > 0)
                    {
                        var rootResult = JsonConvert.DeserializeObject<MatchData>(result);
                        return rootResult;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<List<MatchReference>> GetMatches(int accountID, int championID)
        {
            string apiURL = baseURL + "match/v3/matchlists/by-account/" + accountID;
            var paramters = "?champion=" + championID;

            using (var Client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                Client.DefaultRequestHeaders.Add("Connection", "close");
                try
                {
                    string repURL = apiURL + paramters + "&api_key=" + APIKey;
                    string result = "";
#if DEBUG_STATIC
                    result = File.OpenText(@"Data\Matches.txt").ReadToEnd();
#else
                    var response = Client.GetAsync(repURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                         result = await response.Content.ReadAsStringAsync();
                    }
#endif

                    if (result.Length > 0)
                    {
                        var rootResult = JsonConvert.DeserializeObject<MatchList>(result);
                        return rootResult.matches;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}

