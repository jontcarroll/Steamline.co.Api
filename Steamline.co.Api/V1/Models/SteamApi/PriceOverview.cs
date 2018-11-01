using Newtonsoft.Json;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class PriceOverview
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("initial")]
        public long Initial { get; set; }

        [JsonProperty("final")]
        public long Final { get; set; }

        [JsonProperty("discount_percent")]
        public long DiscountPercent { get; set; }
    }
}