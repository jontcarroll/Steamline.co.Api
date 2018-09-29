using System.Collections.Generic;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.GameFinder;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface IGameFinderService
    {
        IServiceResult<List<GameModel>, ApiErrorModel> GetGamesFromProfileUrl(string url, string groupCode);
    }
}