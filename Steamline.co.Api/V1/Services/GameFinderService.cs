using System.IO;
using System.Collections.Generic;
using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Models.GameFinder;
using System;
using Steamline.co.Api.V1.Services.Utils;
using System.Threading.Tasks;

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

        public async Task<IServiceResult<GameModel, ApiErrorModel>> GetGameDetails(long appId)
        {

            var apiResult = _steamService.GetGameDetails(appId);

            if (apiResult == null) 
            {
                throw new Exception("");
            }

            var retVal = new GameModel() {

            };

            return ServiceResultFactory.Ok<GameModel, ApiErrorModel>(retVal);
        }

        public async Task<IServiceResult<List<GameModel>, ApiErrorModel>> GetGamesFromProfileUrl(string url, string groupCode)
        {
            var steamId = string.Empty;
            try
            {
                steamId = await _steamService.Get64BitSteamId(url);
                _logger.LogDebug($"SteamID for {url}: {steamId}");
            }
            catch(Exception ex)
            {
                //Type of error that is expected and should be relayed back to the client as a toast message
                _logger.LogError(ex, $"Failed to find Steam Id for {url}");
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No Steam profile found for {url}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };
                return ServiceResultFactory.Error<List<GameModel>, ApiErrorModel>(em);
            }

            var games = await _steamService.GetGamesFromProfile(steamId);
            //Example of an error that is unexpected and should not be displayed in the browser
            if (games == null)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"Failed to find games for Steam Id: {steamId}" },
                    Type = ApiErrorModel.TYPE_SILENT_ERROR
                };
                return ServiceResultFactory.Error<List<GameModel>, ApiErrorModel>(em);
            }

            //Type of error that is expected and should be relayed back to the client as a toast message
            if (games.Count == 0)
            {
                var em = new ApiErrorModel()
                {
                    Errors = new List<string>() { $"No games found for Steam Id: {steamId}" },
                    Type = ApiErrorModel.TYPE_TOAST_ERROR
                };

                return ServiceResultFactory.Error<List<GameModel>, ApiErrorModel>(em);
            }

            var retVal = new List<GameModel>();
            foreach (var game in games)
            {
                retVal.Add(new GameModel()
                {
                    AppId = game.AppId,
                    Name = game.Name,
                    PlaytimeForever = game.PlaytimeForever,
                    ImgIconUrl = game.ImgIconUrl,
                    ImgLogoUrl = game.ImgLogoUrl,
                    HasCommunityVisibleStats = game.HasCommunityVisibleStats,
                    Playtime2Weeks = game.Playtime2Weeks
                });
            }


            return ServiceResultFactory.Ok<List<GameModel>, ApiErrorModel>(retVal);
        }

    }
}