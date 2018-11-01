using Steamline.co.Api.V1.Services.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ICustomWebSocketFactory
    {
        void Add(CustomWebSocket uws);
        void Remove(Guid groupIdentifier, string userId);
        List<CustomWebSocket> All();
        List<CustomWebSocket> AllInGroup(CustomWebSocket client);
        CustomWebSocket Client(Guid groupIdentifier, string userId);
    }
}
