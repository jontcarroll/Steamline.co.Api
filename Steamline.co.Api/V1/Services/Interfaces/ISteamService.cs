using System.Collections.Generic;
using System.Threading.Tasks;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ISteamService
    {
        Task<string> Get64BitSteamIdAsync(string profileUrl);
        Task<List<Game>> GetGamesFromProfileAsync(string steamId64);
        Task<GameDetails> GetGameDetailsAsync(long appId);
        Task<List<App>> GetAllAppsAsync();
        Task<List<string>> GetTagsFromGameAsync(long appId);
    }
}