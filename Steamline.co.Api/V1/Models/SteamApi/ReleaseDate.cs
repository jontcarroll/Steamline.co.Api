using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class ReleaseDate
    {
        [JsonProperty("coming_soon")]
        public bool ComingSoon { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }
}