using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class Platforms
    {
        [JsonProperty("windows")]
        public bool Windows { get; set; }

        [JsonProperty("mac")]
        public bool Mac { get; set; }

        [JsonProperty("linux")]
        public bool Linux { get; set; }
    }
}