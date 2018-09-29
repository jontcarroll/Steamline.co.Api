namespace Steamline.co.Api.V1.Models.GameFinder
{
    public partial class GameModel
    {
        public long AppId { get; set; }
        public string Name { get; set; }
        public long PlaytimeForever { get; set; }
        public string ImgIconUrl { get; set; }
        public string ImgLogoUrl { get; set; }
        public bool HasCommunityVisibleStats { get; set; }
        public long? Playtime2Weeks { get; set; }
    }
}