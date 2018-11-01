using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Websocket
{
    public class CustomWebSocket
    {
        public WebSocket WebSocket { get; set; }
        public string UserId { get; set; }
        public string FriendlyName { get; set; }
        public Guid GroupId { get; set; }
    }
}
