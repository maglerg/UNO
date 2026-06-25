using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Shared_Library;

namespace ClientUNO
{
    class GameClient
    {
        public WebSocket Socket { get; set; }
        public Player player { get; set; }

        public GameClient(WebSocket socket)
        {
            Socket = socket;
        }
        public async Task ReceiveLoop()
        {
            while (Socket.State == WebSocketState.Open)
            {
                ServerMessage msg = await ReceiveAsync();

                await GameViewModel.ProcessMessage(msg);
            }
        }
        public async Task<ServerMessage> ReceiveAsync()
        {
            byte[] buffer = new byte[4096];
            WebSocketReceiveResult result =
                await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            JsonDocument doc = JsonDocument.Parse(json);
            string type = doc.RootElement.GetProperty("Type").GetString();

            ServerMessage msg;

            switch (type)
            {
                case "Error":
                    msg = JsonSerializer.Deserialize<ErrorMessage>(json);
                    break;

                case "GameState":
                    msg = JsonSerializer.Deserialize<GameStateMessage>(json);
                    break;
                default:
                    throw new Exception("Unknown message type");
            }
            return msg;
        }

        public async Task SendAsync(object message)
        {
            string json = JsonSerializer.Serialize(message);

            byte[] data = Encoding.UTF8.GetBytes(json);

            await Socket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
