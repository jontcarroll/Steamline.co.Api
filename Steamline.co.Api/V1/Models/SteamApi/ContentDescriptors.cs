using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class ContentDescriptors
    {
        [JsonProperty("ids")]
        public long[] Ids { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }
}