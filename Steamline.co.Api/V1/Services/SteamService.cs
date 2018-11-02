using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamline.co.Api.V1.Config;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services
{
    public class SteamService : ISteamService
    {
        private readonly ILogger<SteamService> _logger;
        private readonly SteamApiConfig _config;
        private readonly Regex _regexTags;

        public SteamService(ILogger<SteamService> logger, IOptions<SteamApiConfig> config)
        {
            _logger = logger;
            _config = config.Value;
            _regexTags = new Regex(@"<a[^>]*class=""app_tag""[^>]*>([^<]*)</a>", RegexOptions.Compiled);
        }

        public async Task<string> Get64BitSteamIdAsync(string profileUrl)
        {
            var uri = new Uri(profileUrl);

            if (!uri.Segments.Any() || uri.Segments.Count() < 2)
                return null;

            // The last parameter will be a SteamId64 or a profile vanity name
            string profileName = uri.Segments.Last().TrimEnd('/');

            // If the link is a profiles link, the last parameter is a SteamId64
            if (uri.Segments.Skip(1).FirstOrDefault()?.ToLower().Contains("profiles") ?? false)
                return profileName;

            var profileResponse = await GetResponseAsync<VanityProfileResponse>(
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

        public async Task<List<Game>> GetGamesFromProfileAsync(string steamId64)
        {
            var gamesResponse = await GetResponseAsync<GameResponse>(
                    _config.ApiSteamUserController,
                    _config.ApiSteamPlayerServiceOwnedGamesAction,
                    "v0001",
                    "include_appinfo=1",
                    "include_played_free_games=1",
                    $"steamid={steamId64}");

            return gamesResponse?.Response?.Games ?? new List<Game>();
        }

        public async Task<IServiceResult<GameDetails, ApiErrorModel>> GetGameDetailsAsync(long appId)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format("https://store.steampowered.com/api/appdetails?appids={0}", appId));
                var response = (HttpWebResponse)await request.GetResponseAsync();
                var reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                var gameDetails = JObject.Parse(jsonResponse).SelectToken(appId.ToString())?.SelectToken("data")?.ToObject<GameDetails>();
                return ServiceResultFactory.Ok<GameDetails, ApiErrorModel>(gameDetails);
            }
            catch (Exception ex)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { "Rate limited", ex.ToString() },
                    Type = ApiErrorModel.TYPE_SERVER_ERROR
                };
                return ServiceResultFactory.Error<GameDetails, ApiErrorModel>(em);
            }
        }

        public async Task<List<App>> GetAllAppsAsync()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_config.GetAllAppsEndpoint);
                var response = (HttpWebResponse)await request.GetResponseAsync();
                var reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                return JObject.Parse(jsonResponse).SelectToken("applist").SelectToken("apps").ToObject<List<App>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private async Task<T> GetResponseAsync<T>(string controller, string action, string apiVersion, params string[] parameters) where T : class
        {
            var parameterList = parameters.ToList();
            parameterList.Insert(0, _config.ApiKeyParameter);
            var request = (HttpWebRequest)WebRequest.Create(
                    string.Format("{0}/{1}/{2}/{3}?{4}",
                    _config.ApiBaseUrl,
                    controller,
                    action,
                    apiVersion,
                    string.Join("&", parameterList)));

            var response = await request.GetResponseAsync();
            var httpResponse = (HttpWebResponse)response;
            var reader = new StreamReader(httpResponse.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        public async Task<List<string>> GetTagsFromGameAsync(long appId)
        {
            HttpWebResponse response = null;
            string storePage = null;

            try
            {
                string storeLanguage = "en";

                var steamRequest = GetSteamRequest(string.Format("http://store.steampowered.com/app/{0}/?l=" + storeLanguage, appId));
                response = await HandleRedirect(steamRequest);

                int count = 0;
                while (response.StatusCode == HttpStatusCode.Found && count < 5)
                {
                    response.Close();
                    // If we are redirected to the store front page
                    if (response.Headers[HttpResponseHeader.Location] == "http://store.steampowered.com/")
                        return new List<string>();

                    //If page redirects to itself
                    if (response.ResponseUri.ToString() == response.Headers[HttpResponseHeader.Location])
                        return new List<string>();

                    steamRequest = GetSteamRequest(response.Headers[HttpResponseHeader.Location]);
                    response = await HandleRedirect(steamRequest);
                    count++;
                }

                //If we got too many redirects
                if (count == 5 && response.StatusCode == HttpStatusCode.Found)
                    return new List<string>();
                // If we were redirected to the store front page
                else if (response.ResponseUri.Segments.Length < 2)
                    return new List<string>();
                // If we encountered an age gate (cookies should bypass this, but sometimes they don't seem to)
                else if (response.ResponseUri.Segments[1] == "agecheck/")
                {
                    if (response.ResponseUri.Segments.Length >= 4 && response.ResponseUri.Segments[3].TrimEnd('/') != appId.ToString())
                    {
                        // Age check + redirect
                        // If we got an age check without numeric id (shouldn't happen)
                        if (!int.TryParse(response.ResponseUri.Segments[3].TrimEnd('/'), out int redirectTarget))
                            return new List<string>();
                    }
                    // If we got an age check with no redirect
                    else
                        return new List<string>();
                }
                // Redirected outside of the app path
                else if (response.ResponseUri.Segments[1] != "app/")
                    return new List<string>();
                // The URI ends with "/app/" ?
                else if (response.ResponseUri.Segments.Length < 3)
                    return new List<string>();
                else if (response.ResponseUri.Segments[2].TrimEnd('/') != appId.ToString())
                {
                    // Redirected to a different app id
                    if (!int.TryParse(response.ResponseUri.Segments[2].TrimEnd('/'), out int redirectTarget))
                        return new List<string>();
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                        return new List<string>();

                    using (var streamReader = new StreamReader(responseStream))
                        storePage = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                response?.Dispose();
            }

            if (storePage == null)
                return new List<string>();

            // Tags
            var matches = _regexTags.Matches(storePage);
            if (matches.Count > 0)
            {
                var tags = new List<string>();
                foreach (Match ma in matches)
                {
                    string tag = WebUtility.HtmlDecode(ma.Groups[1].Value.Trim());
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        tags.Add(tag);
                    }
                }
                return tags;
            }
            return new List<string>();
        }

        private static async Task<HttpWebResponse> HandleRedirect(HttpWebRequest steamRequest)
        {
            try
            {
                return (HttpWebResponse)await steamRequest.GetResponseAsync();
            }
            catch (WebException e)
            {
                if (e.Message.Contains("302"))
                    return (HttpWebResponse)e.Response;
                else
                    throw;
            }
        }

        private HttpWebRequest GetSteamRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            // Cookie bypasses the age gate
            req.CookieContainer = new CookieContainer(3);
            req.CookieContainer.Add(new Cookie("birthtime", "-473392799", "/", "store.steampowered.com"));
            req.CookieContainer.Add(new Cookie("mature_content", "1", "/", "store.steampowered.com"));
            req.CookieContainer.Add(new Cookie("lastagecheckage", "1-January-1955", "/", "store.steampowered.com"));
            // Cookies get discarded on automatic redirects so we have to follow them manually
            req.AllowAutoRedirect = false;
            return req;
        }
    }
}