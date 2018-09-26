using System.IO;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Steamline.co.Api.V1.Services
{
    public class GameFinderService : IGameFinderService
    {
        ILogger<GameFinderService> _logger;
        ISteamService _steamService;

        public GameFinderService(ILogger<GameFinderService> logger, ISteamService steamService)
        {
            _logger = logger;
            _steamService = steamService;
        }

        public IServiceResult<ApiErrorModel> GetGamesFromProfileUrl(string url, string groupCode)
        {
            var steamId = _steamService.Get64BitSteamId(url);
            var games = _steamService.GetGamesFromProfile(steamId);

            return ServiceResultFactory.Ok<ApiErrorModel>();
        }

    }
}