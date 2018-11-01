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
        public async Task SendInitialMessages(CustomWebSocket userWebSocket)
        {
            var webSocket = userWebSocket.WebSocket;
            var msg = new CustomWebSocketMessage
            {
                MessageDateTime = DateTime.Now,
                Type = WebSocketMessageType.Text,
                Text = "",
                Username = "system"
            };

            string serialisedMessage = JsonConvert.SerializeObject(msg);
            byte[] bytes = Encoding.ASCII.GetBytes(serialisedMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var msg = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            try
            {
                var message = new CustomWebSocketMessage()
                {
                    MessageDateTime = DateTime.Now,
                    Type = WebSocketMessageType.Text,
                    Text = msg,
                    Username = userWebSocket.UserId
                };

                var output = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));

                await BroadcastInGroup(output, userWebSocket, wsFactory);
            }
            catch (Exception)
            {
                await userWebSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
        }

        public async Task BroadcastInGroup(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var others = wsFactory.AllInGroup(userWebSocket);
            foreach (var uws in others)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task BroadcastAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var all = wsFactory.All();
            foreach (var uws in all)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
