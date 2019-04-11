using Steamline.co.Api.V1.Services.Websocket;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface ICustomWebSocketMessageHandler
    {
        Task SendInitialMessageAsync(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task SendDisconnectMessageAsync(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task HandleMessageAsync(WebSocketReceiveResult result, IEnumerable<byte> buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastInGroupAsync(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastAllAsync(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
    }

}
