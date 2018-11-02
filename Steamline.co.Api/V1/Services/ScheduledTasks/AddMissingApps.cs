using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.ScheduledTasks
{
    public class AddMissingApps : IScheduledTask
    {
        private readonly ISteamService _steamService;
        private readonly GameSearchService _searchService;
        public AddMissingApps(ISteamService steamService, GameSearchService searchService)
        {
            _steamService = steamService;
            _searchService = searchService;
        }

        // At 4 AM every day
        public string Schedule => "0 4 * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var allApps = await _steamService.GetAllAppsAsync();
            var gameDetails = allApps.Select(a => new GameDetails { Id = a.AppId, Name = a.Name, LastUpdated = DateTime.MinValue });
            await _searchService.AddAppsAsync(gameDetails);
        }
    }
}
