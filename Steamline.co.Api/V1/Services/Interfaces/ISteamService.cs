using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.SteamApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ISteamService
    {
        Task<string> Get64BitSteamIdAsync(string profileUrl);
        Task<List<Game>> GetGamesFromProfileAsync(string steamId64);
        Task<IServiceResult<GameDetails, ApiErrorModel>> GetGameDetailsAsync(long appId);
        Task<List<App>> GetAllAppsAsync();
        Task<List<string>> GetTagsFromGameAsync(long appId);
    }
}