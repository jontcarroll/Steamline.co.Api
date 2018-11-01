using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Websocket
{
    public class CustomWebSocketGroup
    {
        public List<CustomWebSocket> WebSockets { get; set; }
        public string FriendlyName { get; set; }
        public Guid GroupId { get; set; }

        public CustomWebSocket Get(string userId)
        {
            return WebSockets.FirstOrDefault(w => w.UserId == userId);
        }
    }
}
