using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Shared_Library;

namespace ClientUNO
{
    public class GameClient
    {
        public WebSocket Socket { get; set; }
        public PlayerInfo player { get; set; }
        public event Action<ServerMessage>? MessageReceived;
        public GameClient(WebSocket socket)
        {
            Socket = socket;
            player = new PlayerInfo();
        }
        public async Task ReceiveLoop()
        {
            try
            {
                while (Socket.State == WebSocketState.Open)
                {
                    ServerMessage msg = await ReceiveAsync();
                    MessageReceived?.Invoke(msg);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
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
                case "ConfirmMessage":
                    msg = JsonSerializer.Deserialize<ConfirmMessage>(json);
                    break;
                case "ErrorMessage":
                    msg = JsonSerializer.Deserialize<ErrorMessage>(json);
                    break;

                case "GameStateMessage":
                    msg = JsonSerializer.Deserialize<GameStateMessage>(json);
                    break;
                case "SomeoneJoinMessage":
                    msg = JsonSerializer.Deserialize<SomeoneJoinMessage>(json);
                    break;
                case "SomeoneLeftMessage":
                    msg = JsonSerializer.Deserialize<SomeoneLeftMessage>(json);
                    break;
                case "ChangeHostMessage":
                    msg = JsonSerializer.Deserialize<ChangeHostMessage>(json);
                    break;
                case "StartGameMessageToClient":
                    msg = JsonSerializer.Deserialize<StartGameMessageToClient>(json);
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
