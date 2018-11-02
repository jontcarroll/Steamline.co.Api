using Newtonsoft.Json;
using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Services.Websocket
{
    public class CustomWebSocketMessageHandler : ICustomWebSocketMessageHandler
    {
        public async Task SendInitialMessageAsync(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var webSocket = userWebSocket.WebSocket;
            var msg = new CustomWebSocketMessage
            {
                MessageDateTime = DateTime.Now,
                Type = WebSocketMessageType.Text,
                Username = userWebSocket.UserId,
                WebSocketMessageType = "onUserJoin"
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);
            byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
            await BroadcastInGroupAsync(bytes, userWebSocket, wsFactory);
        }

        public async Task SendDisconnectMessageAsync(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var webSocket = userWebSocket.WebSocket;
            var msg = new CustomWebSocketMessage
            {
                MessageDateTime = DateTime.Now,
                Type = WebSocketMessageType.Text,
                Username = userWebSocket.UserId,
                WebSocketMessageType = "onUserLeave"
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);
            byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
            await BroadcastInGroupAsync(bytes, userWebSocket, wsFactory);
        }

        public async Task HandleMessageAsync(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var msg = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            try
            {
                var incomingMessage = JsonConvert.DeserializeObject<IncomingWebSocketMessageWrapper>(msg);

                var message = new CustomWebSocketMessage()
                {
                    MessageDateTime = DateTime.Now,
                    Type = WebSocketMessageType.Text,
                    Username = userWebSocket.UserId,
                    Message = incomingMessage.Message,
                    WebSocketMessageType = incomingMessage.WebSocketMessageType,
                };

                var output = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));

                await BroadcastInGroupAsync(output, userWebSocket, wsFactory);
            }
            catch (Exception)
            {
                await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
        }

        public async Task BroadcastInGroupAsync(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var others = wsFactory.AllInGroup(userWebSocket);
            foreach (var uws in others)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task BroadcastAllAsync(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var all = wsFactory.All();
            foreach (var uws in all)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
