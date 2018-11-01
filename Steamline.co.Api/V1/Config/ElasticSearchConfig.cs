using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Config
{
    public class ElasticSearchConfig
    {
        public string DefaultIndex { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
