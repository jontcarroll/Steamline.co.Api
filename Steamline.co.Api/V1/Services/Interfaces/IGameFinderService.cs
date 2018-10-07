using System.Collections.Generic;
using System.Threading.Tasks;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.GameFinder;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface IGameFinderService
    {
        Task<IServiceResult<List<GameModel>, ApiErrorModel>> GetGamesFromProfileUrl(string url, string groupCode);
        Task<IServiceResult<GameModel, ApiErrorModel>> GetGameDetails(long appId);


    }
}