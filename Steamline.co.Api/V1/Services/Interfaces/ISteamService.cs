using System.Collections.Generic;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ISteamService
    {
        string Get64BitSteamId(string profileUrl);
        List<Game> GetGamesFromProfile(string steamId64);
        GameDetails GetGameDetails(long appId);
    }
}