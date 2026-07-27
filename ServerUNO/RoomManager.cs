using Server;
using Shared_Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerUNO
{
    public class RoomManager
    {
        private readonly Dictionary<string, GameRoom> Rooms = new();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly AccountManager accountManager;
        public RoomManager(AccountManager accountManager)
        {
            this.accountManager = accountManager?? throw new ArgumentNullException(nameof(accountManager));
        }

        public async Task ProcessMessage(ClientConnection client, ServerMessage msg)
        {
            switch (msg)
            {
                case SignUpMessage sMsg:
                    var user = accountManager.Find(sMsg.name, sMsg.password);

                    if (user != null)
                    {
                        await client.SendAsync(new ErrorMessage(
                            RequestType.SignUp,
                            "User already exists"));
                        Console.WriteLine($"User already exists{sMsg.name}");
                    }
                    else
                    {
                        accountManager.Add(new UserAccount(client.Player.Id, sMsg.name, sMsg.password, sMsg.avatarId, ""));
                        accountManager.Save();
                        client.Player.Name = sMsg.name;
                        client.Player.AvatarId = sMsg.avatarId;
                        client.Player.Status = "";
                        var pl = new PlayerInfo(client.Player.Id, client.Player.Name, -1, false, client.Player.Status, client.Player.AvatarId);
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.SignUp,
                            "", pl));

                        Console.WriteLine($"Successful registration {sMsg.name}");
                    }

                    break;
                case LoginMessage lMsg:
                    var user1 = accountManager.Find(lMsg.name, lMsg.password);

                    if (user1 == null)
                    {
                        await client.SendAsync(new ErrorMessage(
                            RequestType.Login,
                            "Invalid username or password."));
                        Console.WriteLine($"User isnt find {lMsg.name}");
                    }
                    else
                    {
                        client.Player.Name = user1.Username;
                        client.Player.AvatarId = user1.AvatarId;
                        client.Player.Status = user1.Status;
                        var pl = new PlayerInfo(user1.Id, user1.Username, -1, false, user1.Status, user1.AvatarId);
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.Login,
                            $"", pl));

                        Console.WriteLine($"User {lMsg.name} logged in");
                    }
                    break;
                case ConnectMessage:
                    try
                    {
                        var room = JoinRoom(msg, client);
                        room.Engine.players.Add(client.Player);
                        client.Room = room;
                        client.Player.IsHost = false;
                        Console.WriteLine($"Clients count = {room.Clients.Count}");

                        var okMsg1 = new ConfirmMessage(RequestType.ConnectRoom, "ok");
                        await client.SendAsync(okMsg1);

                        var newPlayer = new PlayerInfo(client.Player.Id, client.Player.Name, -1,client.Player.IsHost, client.Player.Status, client.Player.AvatarId);
                        
                        foreach (var other in room.Clients) // отправляем остальным, что кто-то присоединился
                        {
                            if (other.Player.Id != client.Player.Id)
                            {
                                Console.WriteLine($"Notify {other.Player.Name} about new player {other.Player.Name}");
                                await other.SendAsync(new SomeoneJoinMessage(newPlayer));
                            }
                        }
                        Console.WriteLine($"User {client.Player.Name} connected");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.ConnectRoom,
                            TextError = ex.Message
                        });
                    }

                    break;
                case RequestPlayersMessage:
                    {
                        var room = client.Room;

                        foreach (var other in room.Clients) // отправляем игроку тех, кто уже есть в комнате
                        {
                            if (other.Player.Id != client.Player.Id)
                            {

                                Console.WriteLine($"Send existing player {other.Player.Name} to new client");
                                var player = new PlayerInfo(client.Player.Id, other.Player.Name, -1, client.Player.IsHost, client.Player.Status, client.Player.AvatarId);
                                await client.SendAsync(new SomeoneJoinMessage(player));
                            }
                        }

                        break;
                    }
                case CreateRoomMessage:
                    try
                    {
                        string code = CreateRoom();
                        GameRoom room = GetRoom(code);
                        room.Clients.Add(client);
                        room.Engine.players.Add(client.Player);
                        client.Room = room;
                        room.HostId = client.Player.Id;
                        client.Player.IsHost = true;
                        var okMsg = new ConfirmMessage(RequestType.CreateRoom, code);
                        await client.SendAsync(okMsg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.CreateRoom,
                            TextError = "Сouldn't create a room"
                        });
                    }

                    break;
                default:
                    client.Room.ProcessMessage(client.Player, msg);
                    break;
            }
            
        }
        string GenerateCode(int length = 9)
        {
            return new string(
                Enumerable.Range(0, length)
                    .Select(_ => chars[Random.Shared.Next(chars.Length)])
                    .ToArray());
        }
        public string CreateRoom()
        {
            GameRoom newRoom = new GameRoom();

            string code;
            do
            {
                code = GenerateCode();
            }
            while (Rooms.ContainsKey(code));

            Rooms.Add(code, newRoom);
            return code;
        }
        public GameRoom GetRoom(string code)
        {
            Rooms.TryGetValue(code, out GameRoom room);
            return room;
        }
        public GameRoom JoinRoom(ServerMessage message, ClientConnection client)
        {
            var connectMsg = message as ConnectMessage;

            if (connectMsg == null)
                throw new Exception("Connect is close");

            string code = connectMsg.CodeRoom;

            GameRoom room = GetRoom(code);
            if (room == null)
                throw new Exception("Room does not exist ");

            if (room.CanJoinPlayer(client.Player))
            {
                room.Clients.Add(client);
                return room;
            }
            throw new Exception("Room is crowded");
        }
        
    }
}
