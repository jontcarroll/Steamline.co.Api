using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class Recommendations
    {
        [JsonProperty("total")]
        public long Total { get; set; }
    }
}