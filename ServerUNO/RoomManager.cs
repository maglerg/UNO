using Server;
using Shared_Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        accountManager.Add(new UserAccount(sMsg.name, sMsg.password));
                        accountManager.Save();
                        client.Player.Name = sMsg.name;
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.SignUp,
                            "ok"));

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
                        Console.WriteLine($"user isnt find {lMsg.name}");
                    }
                    else
                    {
                        Console.WriteLine($"Socket state → {client.Socket?.State}");
                        client.Player.Name = lMsg.name;
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.Login,
                            "ok"));

                        Console.WriteLine($"User {lMsg.name} logged in");
                    }
                    break;
                case ConnectMessage:
                    try
                    {
                        var room = JoinRoom(msg, client);
                        
                        var okMsg1 = new ConfirmMessage(RequestType.ConnectRoom, "ok");
                        await client.SendAsync(okMsg1);
                        for (int i = 0; i < room.Clients.Count; i++)
                        {
                            if (room.Clients[i] != client)
                            {
                                var player = new PlayerInfo(room.Clients[i].Player.Name, -1);
                                await client.SendAsync(new SomeoneJoinMessage(player));
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
                            TextError = "Сouldn't join the room"
                        });
                    }

                    break;
                case CreateRoomMessage:
                    try
                    {
                        string code = CreateRoom();
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

            if (room.CanJoinPlayer(client.Player))
            {
                room.Clients.Add(client);
                return room;
            }
            throw new Exception("Сouldn't find the room");
        }
        
    }
}
