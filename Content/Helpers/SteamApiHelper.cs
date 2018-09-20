using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamline.co.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Steamline.co.Content.Helpers
{
    public static class SteamApiHelper
    {
        private const string API_KEY = "35B05EDE3B3A05EEF1233E352EEB6AA9";
        private const string API_BASE_URL = "http://api.steampowered.com";

        private const string API_STEAM_USER_CONTROLLER = "ISteamUser";
        private const string API_STEAM_USER_VANITY_URL_ACTION = "ResolveVanityURL";

        private const string API_STEAM_PLAYER_SERVICE_CONTROLLER = "IPlayerService";
        private const string API_STEAM_PLAYER_SERVICE_OWNED_GAMES_ACTION = "GetOwnedGames";

        private static string ApiKeyParameter = string.Format("key={0}", API_KEY);

        public static string Get64BitSteamId(string profileUrl)
        {
            var uri = new Uri(profileUrl);

            if (!uri.Segments.Any() || uri.Segments.Count() < 2)
                return null;

            // The last parameter will be a SteamId64 or a profile vanity name
            var profileName = uri.Segments.Last().TrimEnd('/');

            // If the link is a profiles link, the last parameter is a SteamId64
            if (uri.Segments.Skip(1).FirstOrDefault()?.ToLower().Contains("profiles") ?? false)
                return profileName;

            var profileResponse = GetResponse<VanityProfileResponse>(API_STEAM_USER_CONTROLLER, API_STEAM_USER_VANITY_URL_ACTION, "v0001", $"vanityurl={profileName}");

            if (profileResponse?.Response == null)
                return null;

            if (!profileResponse.Response.Success)
                return null;

            return profileResponse.Response.SteamId;

        }

        public static List<Game> GetGamesFromProfile(string steamId64)
        {
            var gamesResponse = GetResponse<GameResponse>(API_STEAM_PLAYER_SERVICE_CONTROLLER, API_STEAM_PLAYER_SERVICE_OWNED_GAMES_ACTION, "v0001", "include_appinfo=1", "include_played_free_games=1", $"steamid={steamId64}");

            return gamesResponse?.Response?.Games ?? new List<Game>();
        }

        public static GameDetails GetGameDetails(long appId)
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

        private static T GetResponse<T>(string controller, string action, string apiVersion, params string[] parameters) where T : class
        {
            var parameterList = parameters.ToList();
            parameterList.Insert(0, ApiKeyParameter);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}/{2}/{3}?{4}", API_BASE_URL, controller, action, apiVersion, string.Join("&", parameterList)));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            catch
            {
                return null;
            }
        }
    }
}