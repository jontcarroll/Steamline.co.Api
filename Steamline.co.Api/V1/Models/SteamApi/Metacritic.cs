using Newtonsoft.Json;
using System;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public partial class Metacritic
    {
        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}