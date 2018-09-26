using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    public class GameResponse
    {
        [JsonProperty("response")]
        public GameResponseContent Response { get; set; }
    }

    public class GameResponseContent
    {
        [JsonProperty("game_count")]
        public long GameCount { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }

    public partial class Game
    {
        [JsonProperty("appid")]
        public long AppId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("playtime_forever")]
        public long PlaytimeForever { get; set; }

        [JsonProperty("img_icon_url")]
        public string ImgIconUrl { get; set; }

        [JsonProperty("img_logo_url")]
        public string ImgLogoUrl { get; set; }

        [JsonProperty("has_community_visible_stats")]
        public bool HasCommunityVisibleStats { get; set; }

        [JsonProperty("playtime_2weeks", NullValueHandling = NullValueHandling.Ignore)]
        public long? Playtime2Weeks { get; set; }
    }

    public static class Serialize
    {
        public static string ToJson(this GameResponse self) => JsonConvert.SerializeObject(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}