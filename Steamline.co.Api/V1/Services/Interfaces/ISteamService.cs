using System.Collections.Generic;
using System.Threading.Tasks;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ISteamService
    {
        Task<string> Get64BitSteamId(string profileUrl);
        Task<List<Game>> GetGamesFromProfile(string steamId64);
        GameDetails GetGameDetails(long appId);
    }
}