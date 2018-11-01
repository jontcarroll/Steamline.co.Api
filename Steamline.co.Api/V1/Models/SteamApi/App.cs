using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public class App
    {
        [JsonProperty("appid")]
        public long AppId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
