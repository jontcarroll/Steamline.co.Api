using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Websocket
{

    public class CustomWebSocketFactory : ICustomWebSocketFactory
    {
        private Dictionary<Guid, CustomWebSocketGroup> WebSockets;

        public CustomWebSocketFactory()
        {
            WebSockets = new Dictionary<Guid, CustomWebSocketGroup>();
        }

        public void Add(CustomWebSocket webSocket)
        {
            if (WebSockets.TryGetValue(webSocket.GroupId, out var value))
            {
                value.WebSockets.Add(webSocket);
            }
            else
            {
                var newWebSocketGroup = new CustomWebSocketGroup() { GroupId = webSocket.GroupId, FriendlyName = webSocket.FriendlyName };
                newWebSocketGroup.WebSockets = new List<CustomWebSocket> { webSocket };
                this.WebSockets.Add(webSocket.GroupId, newWebSocketGroup);
            }
        }

        public void Remove(Guid groupIdentifier, string userId)
        {
            WebSockets[groupIdentifier].WebSockets.Remove(Client(groupIdentifier, userId));
            if(!WebSockets[groupIdentifier].WebSockets.Any())
            {
                WebSockets.Remove(groupIdentifier);
            }
        }

        public List<CustomWebSocket> All()
        {
            return WebSockets.SelectMany(w => w.Value.WebSockets).ToList();
        }

        public List<CustomWebSocket> AllInGroup(CustomWebSocket client)
        {
            return WebSockets[client.GroupId].WebSockets;
        }

        public CustomWebSocket Client(Guid groupIdentifier, string userId)
        {
            return WebSockets[groupIdentifier].WebSockets.First(c => c.UserId == userId);
        }
    }
}
