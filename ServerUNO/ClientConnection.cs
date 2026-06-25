using Server;
using ServerUNO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared_Library
{
    public class ClientConnection
    {
        public WebSocket Socket { get; set; }
        public Player Player { get; set; }
        public GameRoom Room { get; set; }
        public RoomManager roomManager { get; set; }
        /*public bool IsConnected { get; set; }*/


        public ClientConnection(WebSocket socket, RoomManager roomManager)
        {
            Socket = socket;
            this.roomManager = roomManager;
        }
        public async Task ReceiveLoop()
        {
            while (Socket.State == WebSocketState.Open)
            {
                ServerMessage msg = await ReceiveAsync();

                await roomManager.ProcessMessage(this, msg);
                // дальше в  RoomManager -> GameRoom -> GameEngine
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
                case "Connect":
                    msg = JsonSerializer.Deserialize<ConnectMessage>(json);
                    break;
                case "CreateRoom":
                    msg = JsonSerializer.Deserialize<CreateRoomMessage>(json);
                    break;
                case "TakeCard":
                    msg = JsonSerializer.Deserialize<TakeCardMessage>(json);
                    break;
                case "PlayCard":
                    msg = JsonSerializer.Deserialize<PlayCardMessage>(json);
                    break;
                case "StartGame":
                    msg = JsonSerializer.Deserialize<StartGameMessage>(json);
                    break;
                case "Error":
                    msg = JsonSerializer.Deserialize<ErrorMessage>(json);
                    break;
                case "PlayUNO":
                    msg = JsonSerializer.Deserialize<PushUNOMessage>(json);
                    break;
                case "GameState":
                    msg = JsonSerializer.Deserialize<GameStateMessage>(json);
                    break;
                case "SignUp":
                    msg = JsonSerializer.Deserialize<SignUpMessage>(json);
                    break;
                case "Login":
                    msg = JsonSerializer.Deserialize<LoginMessage>(json);
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

        //State()   проверить состояние
    }
}

