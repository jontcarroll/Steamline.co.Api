using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.ScheduledTasks
{
    public class UpdateAppDetails : IScheduledTask
    {
        private readonly ISteamService _steamService;
        private readonly GameSearchService _searchService;
        public UpdateAppDetails(ISteamService steamService, GameSearchService searchService)
        {
            _steamService = steamService;
            _searchService = searchService;
        }

        public string Schedule => "*/1 * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var documents = await _searchService.GetOldestAppDetailsAsync(150);

            foreach (var game in documents)
            {
                var detailResponse = await _steamService.GetGameDetailsAsync(game.Id);
                if (detailResponse.HasError)
                {
                    if (detailResponse.Error.Type == ApiErrorModel.TYPE_SERVER_ERROR)
                        break;
                    else
                        continue;
                }

                var details = detailResponse.Value ?? game;

                if (details.Type == "game")
                {
                    var tags = await _steamService.GetTagsFromGameAsync(game.Id);
                    details.UserDefinedTags = tags;
                }
                else
                {
                    // Don't want to waste time and API calls updating non-game items
                    details.LastUpdated = DateTime.MaxValue;
                }

                await _searchService.AddGameDetailsAsync(details);
            }

        }
    }
}
