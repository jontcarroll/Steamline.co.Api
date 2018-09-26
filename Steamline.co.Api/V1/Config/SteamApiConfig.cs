namespace Steamline.co.Api.V1.Config
{
    public class SteamApiConfig
    {
        public string ApiKey { get; set; }
        public string ApiBaseUrl { get; set; }

        public string ApiSteamUserController { get; set; }
        public string ApiSteamUserVanityUrlAction { get; set; }
        
        public string ApiSteamPlayerServiceController { get; set; }
        public string ApiSteamPlayerServiceOwnedGamesAction { get; set; }

        public string ApiKeyParameter
        {
            get { return $"key={ApiKey}"; }
        }
    }
}