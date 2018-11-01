using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steamline.co.Api.V1.Models;

namespace Steamline.co.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/gamefinder")]
    public class GameFinder : Controller
    {
        IGameFinderService _gameFinderService;
        public GameFinder(IGameFinderService gameFinderService) {
            _gameFinderService = gameFinderService;
        }

        [HttpGet("{groupCode}/{url}")]
        public async Task<ServiceActionResult> GetGamesFromProfileUrl(string url)
        {
            var result = await _gameFinderService.GetGamesFromProfileUrl(url);
            return ServiceActionResultFactory.Create(result);
        }

        [HttpGet("gamedetails/{appId}")]
        public async Task<ServiceActionResult> GetGameDetails(long appId)
        {
            var result = await _gameFinderService.GetGameDetails(appId);
            return ServiceActionResultFactory.Create(result);
        }
    }
}