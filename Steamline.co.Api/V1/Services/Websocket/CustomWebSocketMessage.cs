using Newtonsoft.Json;
using System;
using System.Net.WebSockets;

namespace Steamline.co.Api.V1.Services.Websocket
{
    public class CustomWebSocketMessage
    {
        public string Text { get; set; }

        public DateTime MessageDateTime { get; set; }
        public string Username { get; set; }
        public WebSocketMessageType Type { get; set; }

        [JsonProperty(PropertyName = "wsMessageType")]
        public string WebSocketMessageType { get; set; }
    }
}
