using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
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
        private readonly ILogger<IScheduledTask> _logger;

        public AddMissingApps(ISteamService steamService, GameSearchService searchService, ILogger<IScheduledTask> logger)
        {
            _steamService = steamService;
            _searchService = searchService;
            _logger = logger;
        }

        // At 4 AM every day
        public string Schedule => "0 4 * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.ScheduledTasks), $"Beginning task: {this.GetType().Name}");
            var allApps = await _steamService.GetAllAppsAsync();
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.ScheduledTasks), $"Apps found: {allApps.Count}");
            var gameDetails = allApps.Select(a => new GameDetails { Id = a.AppId, Name = a.Name, LastUpdated = DateTime.MinValue });
            await _searchService.AddAppsAsync(gameDetails);
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.ScheduledTasks), $"Finishing task: {GetType().Name}");
        }
    }
}
