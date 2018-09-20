using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Steamline.co.Models
{
    public class VanityProfileResponse
    {
        [JsonProperty("response")]
        public ProfileResponse Response { get; set; }
    }

    public class ProfileResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("steamid")]
        public string SteamId { get; set; }
    }
}