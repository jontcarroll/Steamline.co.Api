using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Models.SteamApi
{
    [ElasticsearchType(IdProperty = "Id")]
    public partial class GameDetails
    {
        [JsonProperty("steam_appid")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("is_free")]
        public bool IsFree { get; set; }

        [JsonProperty("controller_support")]
        public string ControllerSupport { get; set; }

        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }

        [JsonProperty("supported_languages")]
        public string SupportedLanguages { get; set; }

        [JsonProperty("header_image")]
        public Uri HeaderImage { get; set; }

        [JsonProperty("website")]
        public Uri Website { get; set; }

        [JsonProperty("price_overview")]
        public PriceOverview PriceOverview { get; set; }

        [JsonProperty("packages")]
        public long[] Packages { get; set; }

        [JsonProperty("platforms")]
        public Platforms Platforms { get; set; }

        [JsonProperty("metacritic")]
        public Metacritic Metacritic { get; set; }

        [JsonProperty("categories")]
        public Category[] Categories { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("recommendations")]
        public Recommendations Recommendations { get; set; }

        [JsonProperty("release_date")]
        public ReleaseDate ReleaseDate { get; set; }

        [JsonProperty("background")]
        public Uri Background { get; set; }

        [JsonProperty("content_descriptors")]
        public ContentDescriptors ContentDescriptors { get; set; }

        public List<string> UserDefinedTags { get; set; }

        [JsonIgnore]
        public Game UserGameInfo { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}