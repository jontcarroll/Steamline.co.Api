using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public class PlayerRootResponse
    {
        [JsonProperty("response")]
        public PlayerResponse Response { get; set; }
    }

    public class PlayerResponse
    {
        [JsonProperty("players")]
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        [JsonProperty("steamid")]
        public string SteamId { get; set; }

        [JsonProperty("communityvisibilitystate")]
        public long CommunityVisibilityState { get; set; }

        [JsonProperty("profilestate")]
        public long ProfileState { get; set; }

        [JsonProperty("personaname")]
        public string PersonaName { get; set; }

        [JsonProperty("lastlogoff")]
        public long LastLogOff { get; set; }

        [JsonProperty("profileurl")]
        public Uri ProfileUrl { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }

        [JsonProperty("avatarmedium")]
        public Uri Avatarmedium { get; set; }

        [JsonProperty("avatarfull")]
        public Uri Avatarfull { get; set; }

        [JsonProperty("personastate")]
        public long PersonaState { get; set; }

        [JsonProperty("realname")]
        public string RealName { get; set; }

        [JsonProperty("primaryclanid")]
        public string PrimaryClanId { get; set; }

        [JsonProperty("timecreated")]
        public long TimeCreated { get; set; }

        [JsonProperty("personastateflags")]
        public long PersonaStateFlags { get; set; }

        [JsonProperty("loccountrycode")]
        public string CountryCode { get; set; }

        [JsonProperty("locstatecode")]
        public string StateCode { get; set; }

        [JsonProperty("loccityid")]
        public long CityId { get; set; }
    }
}
