using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.ScheduledTasks
{
    public class UpdateAppDetails : IScheduledTask
    {
        private readonly ISteamService _steamService;
        private readonly GameSearchService _searchService;
        private readonly ILogger<IScheduledTask> _logger;

        public UpdateAppDetails(ISteamService steamService, GameSearchService searchService, ILogger<IScheduledTask> logger)
        {
            _steamService = steamService;
            _searchService = searchService;
            _logger = logger;
        }

        // Every 6 minutes
        public string Schedule => "*/6 * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.ScheduledTasks), $"Beginning task: {GetType().Name}");
            var documents = await _searchService.GetOldestAppDetailsAsync(150);

            foreach (var game in documents)
            {
                var detailResponse = await _steamService.GetGameDetailsAsync(game.Id);
                if (detailResponse.HasError)
                {
                    if (detailResponse.Error.Errors.Any(e => e.Contains("Rate limit")))
                    {
                        _logger.Log(LogLevel.Warning, new EventId((int)LogEventId.ScheduledTasks), "Rate limit reached");
                        break;
                    }
                    else
                    {
                        foreach (string error in detailResponse.Error.Errors)
                        {
                            _logger.Log(LogLevel.Error, new EventId((int)LogEventId.ScheduledTasks), error);
                        }
                        continue;
                    }
                }

                var details = detailResponse.Value ?? game;

                if (details.Type == "game")
                {
                    var tags = await _steamService.GetTagsFromGameAsync(game.Id);
                    if (tags != null && tags.Any())
                        details.UserDefinedTags = tags;
                    else
                        _logger.Log(LogLevel.Warning, new EventId((int)LogEventId.ScheduledTasks), $"No tags found for: {game.Id} - {game.Name}");

                }
                else if (!string.IsNullOrEmpty(details.Type))
                {
                    // Don't want to waste time and API calls updating non-game items
                    details.LastUpdated = DateTime.MaxValue;
                }
                else
                {
                    _logger.Log(LogLevel.Warning, new EventId((int)LogEventId.ScheduledTasks), $"No details found for: {game.Id} - {game.Name}");
                    details.LastUpdated = DateTime.UtcNow;
                }

                await _searchService.AddGameDetailsAsync(details);
            }
            _logger.Log(LogLevel.Information, new EventId((int)LogEventId.ScheduledTasks), $"Finishing task: {GetType().Name}");

        }
    }
}
