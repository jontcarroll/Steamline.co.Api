using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Steamline.co.Api.V1.Config;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services
{
    public class GameSearchService
    {
        private ElasticSearchConfig _config;
        private ElasticService _client;
        private IWorkerQueue _workerQueue;
        private ILogger<GameSearchService> _logger;

        public GameSearchService(IOptions<ElasticSearchConfig> config, ElasticService client, IWorkerQueue workerQueue, ILogger<GameSearchService> logger)
        {
            _config = config.Value;
            _client = client;
            _workerQueue = workerQueue;
            _logger = logger;
        }

        public async Task AddGameDetailsAsync(GameDetails game)
        {
            await _client.IndexDocumentAsync(game);
        }

        public async Task AddAppsAsync(IEnumerable<GameDetails> apps)
        {
            foreach (var app in apps)
            {
                var response = await _client.CreateDocumentAsync(app);
                if (response.Result != Result.Error)
                    _logger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, new EventId((int)LogEventId.General), $"Added App ID {app.Id}");
            }
        }

        public async Task<List<GameDetails>> GetAsync(params long[] appIds)
        {
            var response = await _client.SearchAsync<GameDetails>(x => x.Query(g => g.Ids(i => i.Values(appIds))));
            return response.Documents.ToList();
        }

        public async Task<List<GameDetails>> GetOldestAppDetailsAsync(int documentCount)
        {
            var response = await _client.SearchAsync<GameDetails>(x => x.Query(g => g).Sort(d => d.Ascending(o => o.LastUpdated)).Size(documentCount));
            return response.Documents.ToList();
        }
    }
}
