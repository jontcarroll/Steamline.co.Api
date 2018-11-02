using System.Collections.Generic;
using System.Threading.Tasks;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.GameFinder;
using Steamline.co.Api.V1.Models.SteamApi;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface IGameFinderService
    {
        Task<IServiceResult<string, ApiErrorModel>> GetSteamIdFromProfileUrl(string url);
        Task<IServiceResult<GameDetails, ApiErrorModel>> GetGameDetails(long appId);
        Task<IServiceResult<List<GameDetails>, ApiErrorModel>> GetGamesForSteamIdAsync(string steamId64);


    }
}