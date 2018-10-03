using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamline.co.Api.V1.Config;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;

namespace Steamline.co.Api.V1.Services
{
    public class SteamService : ISteamService
    {
        ILogger<SteamService> _logger;
        SteamApiConfig _config;
        public SteamService(ILogger<SteamService> logger, IOptions<SteamApiConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public string Get64BitSteamId(string profileUrl)
        {
            var uri = new Uri(profileUrl);

            if (!uri.Segments.Any() || uri.Segments.Count() < 2)
                return null;

            // The last parameter will be a SteamId64 or a profile vanity name
            var profileName = uri.Segments.Last().TrimEnd('/');

            // If the link is a profiles link, the last parameter is a SteamId64
            if (uri.Segments.Skip(1).FirstOrDefault()?.ToLower().Contains("profiles") ?? false)
                return profileName;

            var profileResponse = GetResponse<VanityProfileResponse>(
                    _config.ApiSteamUserController,
                    _config.ApiSteamUserVanityUrlAction, 
                    "v0001", 
                    $"vanityurl={profileName}");

            if (profileResponse?.Response == null)
                return null;

            if (!profileResponse.Response.Success)
                return null;

            return profileResponse.Response.SteamId;

        }

        public List<Game> GetGamesFromProfile(string steamId64)
        {
            var gamesResponse = GetResponse<GameResponse>(
                    _config.ApiSteamUserController,
                    _config.ApiSteamPlayerServiceOwnedGamesAction, 
                    "v0001", 
                    "include_appinfo=1", 
                    "include_played_free_games=1", 
                    $"steamid={steamId64}");

            return gamesResponse?.Response?.Games ?? new List<Game>();
        }

        public GameDetails GetGameDetails(long appId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://store.steampowered.com/api/appdetails?appids={0}", appId));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                return JObject.Parse(jsonResponse).SelectToken(appId.ToString())?.SelectToken("data")?.ToObject<GameDetails>();
            }
            catch
            {
                return null;
            }
        }

        private T GetResponse<T>(string controller, string action, string apiVersion, params string[] parameters) where T : class
        {
            var parameterList = parameters.ToList();
            parameterList.Insert(0, _config.ApiKeyParameter);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                        string.Format("{0}/{1}/{2}/{3}?{4}",
                        _config.ApiBaseUrl, 
                        controller, 
                        action, 
                        apiVersion, 
                        string.Join("&", parameterList)));
                        
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}