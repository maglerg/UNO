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


        public ClientConnection(WebSocket socket, RoomManager roomManager)
        {
            Socket = socket;
            this.roomManager = roomManager;
            Player = new Player();
            Room = new GameRoom();
        }
        public async Task ReceiveLoop()
        {
            while (Socket.State == WebSocketState.Open)
            {
                try
                {
                    ServerMessage msg = await ReceiveAsync();
                    Console.WriteLine($"RECEIVED: {msg?.GetType().Name}");
                    await roomManager.ProcessMessage(this, msg);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
        public async Task<ServerMessage> ReceiveAsync()
        {
            byte[] buffer = new byte[4096];

            WebSocketReceiveResult result =
                await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var baseMsg = JsonSerializer.Deserialize<JsonElement>(json);
            var type = baseMsg.GetProperty("Type").GetString();

            if (type == null)
                throw new Exception("Message Type is null");

            return type switch
            {
                nameof(ConnectMessage) => JsonSerializer.Deserialize<ConnectMessage>(json),
                nameof(CreateRoomMessage) => JsonSerializer.Deserialize<CreateRoomMessage>(json),
                nameof(TakeCardMessage) => JsonSerializer.Deserialize<TakeCardMessage>(json),
                nameof(PlayCardMessage) => JsonSerializer.Deserialize<PlayCardMessage>(json),
                nameof(CanStartGameMessage) => JsonSerializer.Deserialize<CanStartGameMessage>(json),
                nameof(ErrorMessage) => JsonSerializer.Deserialize<ErrorMessage>(json),
                nameof(PushUNOMessage) => JsonSerializer.Deserialize<PushUNOMessage>(json),
                nameof(GameStateMessage) => JsonSerializer.Deserialize<GameStateMessage>(json),
                nameof(SignUpMessage) => JsonSerializer.Deserialize<SignUpMessage>(json),
                nameof(LoginMessage) => JsonSerializer.Deserialize<LoginMessage>(json),
                nameof(SomeoneJoinMessage) => JsonSerializer.Deserialize<SomeoneJoinMessage>(json),
                nameof(SomeoneLeftMessage) => JsonSerializer.Deserialize<SomeoneLeftMessage>(json),
                nameof(LeaveGameMessage) => JsonSerializer.Deserialize<LeaveGameMessage>(json),
                nameof(RequestPlayersMessage) => JsonSerializer.Deserialize<RequestPlayersMessage>(json),
                nameof(StartGameMessageToServer) => JsonSerializer.Deserialize<StartGameMessageToServer>(json),
                

                _ => throw new Exception($"Unknown message type: {type}")
            };
        }

        public async Task SendAsync(object message)
        {
            string json = JsonSerializer.Serialize(message);

            byte[] data = Encoding.UTF8.GetBytes(json);
            Console.WriteLine("OUT → " + json);
            await Socket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        //State()   проверить состояние
    }
}

