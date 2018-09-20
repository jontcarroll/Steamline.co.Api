using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Steamline.co.Models
{
    public partial class GameDetails
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("steam_appid")]
        public long SteamAppId { get; set; }

        [JsonProperty("required_age")]
        public long RequiredAge { get; set; }

        [JsonProperty("is_free")]
        public bool IsFree { get; set; }

        [JsonProperty("controller_support")]
        public string ControllerSupport { get; set; }

        [JsonProperty("detailed_description")]
        public string DetailedDescription { get; set; }

        [JsonProperty("about_the_game")]
        public string AboutTheGame { get; set; }

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

        [JsonIgnore]
        public Game UserGameInfo { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class ContentDescriptors
    {
        [JsonProperty("ids")]
        public long[] Ids { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    public partial class Genre
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class Metacritic
    {
        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Platforms
    {
        [JsonProperty("windows")]
        public bool Windows { get; set; }

        [JsonProperty("mac")]
        public bool Mac { get; set; }

        [JsonProperty("linux")]
        public bool Linux { get; set; }
    }

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

    public partial class Recommendations
    {
        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class ReleaseDate
    {
        [JsonProperty("coming_soon")]
        public bool ComingSoon { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }


}