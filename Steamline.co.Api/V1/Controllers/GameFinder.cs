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
        public ServiceActionResult GetGamesFromProfileUrl(string groupCode, string url)
        {
            //Need to handle encoding from client, for now just doing this
            url = $"https://steamcommunity.com/profiles/{url}";
            var result = _gameFinderService.GetGamesFromProfileUrl(url, groupCode);
            return ServiceActionResultFactory.Create(result);
        }
    }
}