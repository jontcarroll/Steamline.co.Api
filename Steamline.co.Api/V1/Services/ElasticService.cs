using Microsoft.Extensions.Options;
using Nest;
using Steamline.co.Api.V1.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services
{
    public class ElasticService : ElasticClient
    {
        private ElasticSearchConfig _config;

        public ElasticService(IOptions<ElasticSearchConfig> config) : base(GetConnectionSettings(config.Value))
        {
            _config = config.Value;
        }


        private static ConnectionSettings GetConnectionSettings(ElasticSearchConfig config)
        {
            return new ConnectionSettings(new Uri($"http://{config.Host}:{config.Port}")).DefaultIndex(config.DefaultIndex);
        }

    }
}
