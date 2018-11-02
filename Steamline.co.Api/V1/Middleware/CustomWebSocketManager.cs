using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services.Websocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Middleware
{

    public class CustomWebSocketManager
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Player> _logger;

        public CustomWebSocketManager(RequestDelegate next, ILogger<Player> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    string groupId = context.Request.Query["g"];
                    string friendlyName = context.Request.Query["f"];
                    string userId = context.Request.Query["u"];
                    if (!string.IsNullOrEmpty(groupId))
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var userWebSocket = new CustomWebSocket()
                        {
                            WebSocket = webSocket,
                            GroupId = Guid.Parse(groupId),
                            FriendlyName = friendlyName,
                            UserId = userId,
                        };
                        wsFactory.Add(userWebSocket);
                        _logger.Log(LogLevel.Information, new EventId((int)LogEventId.User), $"User {userId} joined group {groupId}");
                        await Listen(context, userWebSocket, wsFactory, wsmHandler);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            await _next(context);
        }

        private async Task Listen(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler)
        {
            var webSocket = userWebSocket.WebSocket;
            byte[] buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await wsmHandler.HandleMessage(result, buffer, userWebSocket, wsFactory);
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            wsFactory.Remove(userWebSocket.GroupId, userWebSocket.UserId);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
