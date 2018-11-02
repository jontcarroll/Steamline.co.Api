using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Websocket
{
    [Serializable]
    public class IncomingWebSocketMessageWrapper
    {
        public string Message { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string WebSocketMessageType { get; set; }
    }
}
