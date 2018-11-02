using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Models;
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
        private readonly ILogger<GameFinderService> _logger;
        private readonly ISteamService _steamService;
        private readonly GameSearchService _gameSearchService;

        public GameFinderService(ILogger<GameFinderService> logger, ISteamService steamService, GameSearchService gameSearchService)
        {
            _logger = logger;
            _steamService = steamService;
            _gameSearchService = gameSearchService;
        }

        public async Task<IServiceResult<GameDetails, ApiErrorModel>> GetGameDetails(long appId)
        {
            var gameDetails = await _steamService.GetGameDetailsAsync(appId);

            if (gameDetails.HasError)
            {
                return gameDetails;
            }

            if (gameDetails.Value == null)
            {
                return ServiceResultFactory.Error<GameDetails, ApiErrorModel>(new ApiErrorModel()
                {
                    Errors = new List<string>() { $"Failed to find game for app id: {appId}" },
                    Type = ApiErrorModel.TYPE_SILENT_ERROR
                });
            }

            return ServiceResultFactory.Ok<GameDetails, ApiErrorModel>(gameDetails.Value);
        }

        public async Task<IServiceResult<string, ApiErrorModel>> GetSteamIdFromProfileUrl(string url)
        {
            string steamId = string.Empty;
            try
            {
                steamId = await _steamService.Get64BitSteamIdAsync(url);
                _logger.Log(LogLevel.Debug, new EventId((int)LogEventId.General), $"SteamID for {url}: {steamId}");
            }
            catch (Exception ex)
            {
                //Type of error that is expected and should be relayed back to the client as a toast message
                _logger.Log(LogLevel.Error, new EventId((int)LogEventId.General), $"Failed to find Steam Id for {url}");
                _logger.Log(LogLevel.Error, new EventId((int)LogEventId.General), ex.ToString());
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No Steam profile found for {url}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };

                return ServiceResultFactory.Error<string, ApiErrorModel>(em);
            }
            return ServiceResultFactory.Ok<string, ApiErrorModel>(steamId);
        }

        public async Task<IServiceResult<List<GameDetails>, ApiErrorModel>> GetGamesForSteamIdAsync(string steamId64)
        {
            var games = await _steamService.GetGamesFromProfileAsync(steamId64);
            //Example of an error that is unexpected and should not be displayed in the browser
            if (games == null)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"Failed to find games for Steam Id: {steamId64}" },
                    Type = ApiErrorModel.TYPE_SILENT_ERROR
                };
                return ServiceResultFactory.Error<List<GameDetails>, ApiErrorModel>(em);
            }

            //Type of error that is expected and should be relayed back to the client as a toast message
            if (games.Count == 0)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No games found for Steam Id: {steamId64}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };

                return ServiceResultFactory.Error<List<GameDetails>, ApiErrorModel>(em);
            }

            var returnValues = await _gameSearchService.GetAsync(games.Select(g => g.AppId).ToArray());


            return ServiceResultFactory.Ok<List<GameDetails>, ApiErrorModel>(returnValues);
        }
    }
}