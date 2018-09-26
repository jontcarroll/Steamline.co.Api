using Steamline.co.Api.V1.Models;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface IGameFinderService
    {
        IServiceResult<ApiErrorModel> GetGamesFromProfileUrl(string url, string groupCode);
    }
}