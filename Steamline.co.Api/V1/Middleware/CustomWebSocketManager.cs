using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Helpers;
using Steamline.co.Api.V1.Models.SteamApi;
using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services.Websocket;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Middleware
{

    public class CustomWebSocketManager
    {
        private readonly RequestDelegate _nextAsync;
        private readonly ILogger<Player> _logger;

        public CustomWebSocketManager(RequestDelegate next, ILogger<Player> logger)
        {
            _nextAsync = next;
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
                        await wsmHandler.SendInitialMessageAsync(userWebSocket, wsFactory);
                        await ListenAsync(context, userWebSocket, wsFactory, wsmHandler);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            await _nextAsync(context);
        }

        async Task<(WebSocketReceiveResult, IEnumerable<byte>)> ReceiveFullMessage(WebSocket socket, CancellationToken cancelToken)
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[4096];
            do
            {
                response = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancelToken);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            return (response, message);
        }

        private async Task ListenAsync(HttpContext context, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory, ICustomWebSocketMessageHandler wsmHandler)
        {
            var webSocket = userWebSocket.WebSocket;
            byte[] buffer = new byte[1024 * 16];
            var message = new List<byte>();
            WebSocketReceiveResult response;
            do
            {
                do
                {
                    response = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
                } while (!response.EndOfMessage);
                await wsmHandler.HandleMessageAsync(response, message, userWebSocket, wsFactory);
            } while (!response.CloseStatus.HasValue);

            await wsmHandler.SendDisconnectMessageAsync(userWebSocket, wsFactory);
            wsFactory.Remove(userWebSocket.GroupId, userWebSocket.UserId);
            await webSocket.CloseAsync(response.CloseStatus.Value, response.CloseStatusDescription, CancellationToken.None);
        }
    }
}
