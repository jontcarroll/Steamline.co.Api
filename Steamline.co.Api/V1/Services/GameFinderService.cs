using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Models.GameFinder;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services
{
    public class GameFinderService : IGameFinderService
    {
        private ILogger<GameFinderService> _logger;
        private ISteamService _steamService;

        public GameFinderService(ILogger<GameFinderService> logger, ISteamService steamService)
        {
            _logger = logger;
            _steamService = steamService;
        }

        public async Task<IServiceResult<GameDetails, ApiErrorModel>> GetGameDetails(long appId)
        {
            var gameDetails = SteamGames.GameDetails[appId] ?? await _steamService.GetGameDetailsAsync(appId);

            if (gameDetails == null)
            {
                return ServiceResultFactory.Error<GameDetails, ApiErrorModel>(new ApiErrorModel()
                {
                    Errors = new List<string>() { $"Failed to find game for app id: {appId}" },
                    Type = ApiErrorModel.TYPE_SILENT_ERROR
                });
            }

            return ServiceResultFactory.Ok<GameDetails, ApiErrorModel>(gameDetails);
        }

        public async Task<IServiceResult<List<GameDetails>, ApiErrorModel>> GetGamesFromProfileUrl(string url)
        {
            string steamId = string.Empty;
            try
            {
                steamId = await _steamService.Get64BitSteamIdAsync(url);
                _logger.LogDebug($"SteamID for {url}: {steamId}");
            }
            catch (Exception ex)
            {
                //Type of error that is expected and should be relayed back to the client as a toast message
                _logger.LogError(ex, $"Failed to find Steam Id for {url}");
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No Steam profile found for {url}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };
                return ServiceResultFactory.Error<List<GameDetails>, ApiErrorModel>(em);
            }

            var games = await _steamService.GetGamesFromProfileAsync(steamId);
            //Example of an error that is unexpected and should not be displayed in the browser
            if (games == null)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"Failed to find games for Steam Id: {steamId}" },
                    Type = ApiErrorModel.TYPE_SILENT_ERROR
                };
                return ServiceResultFactory.Error<List<GameDetails>, ApiErrorModel>(em);
            }

            //Type of error that is expected and should be relayed back to the client as a toast message
            if (games.Count == 0)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No games found for Steam Id: {steamId}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };

                return ServiceResultFactory.Error<List<GameDetails>, ApiErrorModel>(em);
            }

            var returnValues = games.Select(g => SteamGames.GameDetails[g.AppId]).ToList();


            return ServiceResultFactory.Ok<List<GameDetails>, ApiErrorModel>(returnValues);
        }

    }
}