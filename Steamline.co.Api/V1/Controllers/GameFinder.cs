using Microsoft.AspNetCore.Mvc;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Services.Interfaces;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/gamefinder")]
    public class GameFinder : Controller
    {
        private readonly IGameFinderService _gameFinderService;
        private readonly ISteamService _steamService;
        public GameFinder(IGameFinderService gameFinderService, ISteamService steamService)
        {
            _gameFinderService = gameFinderService;
        }

        [HttpGet("getGamesFromProfileUrl/{url}")]
        public async Task<ServiceActionResult> GetGamesFromProfileUrl(string url)
        {
            var result = await _gameFinderService.GetGamesFromProfileUrlAsync(url);
            return ServiceActionResultFactory.Create(result);
        }

        [HttpGet("gamedetails/{appId}")]
        public async Task<ServiceActionResult> GetGameDetails(long appId)
        {
            var result = await _gameFinderService.GetGameDetails(appId);
            return ServiceActionResultFactory.Create(result);
        }

        [HttpGet("GetUserDetails")]
        public async Task<ServiceActionResult> GetUserDetails(string[] userIds)
        {
            var result = await _steamService.GetPlayersAsync(userIds);
            return ServiceActionResultFactory.Create(result);
        }
    }
}