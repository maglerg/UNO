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
        private int MaxAvatar = 6;
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
                        var newAcc = new UserAccount(sMsg.name, sMsg.password, sMsg.name, sMsg.avatarId);
                        accountManager.Add(newAcc);
                        client.Player.Name = sMsg.name;
                        client.Player.AvatarId = sMsg.avatarId;
                        client.Player.Status = "";
                        client.userAcc = newAcc;
                        var pl = new PlayerInfo(client.Player.Id, client.Player.Name, -1, false, client.Player.Status, client.Player.AvatarId, client.Player.Wins, client.Player.CountGames, client.Player.Points);
                        
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
                        client.Player.Id = user1.Id;
                        client.Player.Name = user1.Name;
                        client.Player.AvatarId = user1.AvatarId;
                        client.Player.Status = user1.Status;
                        client.Player.Wins = user1.Wins;
                        client.Player.CountGames = user1.CountGames;
                        client.Player.Points = 0;
                        client.userAcc = user1;
                        var pl = new PlayerInfo(user1.Id, user1.Name, -1, false, user1.Status, user1.AvatarId, user1.Wins, user1.CountGames, 0);
                        
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.Login,
                            $"", pl));

                        Console.WriteLine($"User {lMsg.name} logged in");
                    }
                    break;
                case EditProfileMessage eMsg:
                    var userAccount = client.userAcc;

                    if (userAccount == null)
                    {
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.EditProfile,
                            TextError = "User not found"
                        });

                        break;
                    }
                    bool isCorrect = true;
                    /*if ( eMsg.name.Length < 3 || eMsg.name.Length > 20)
                        isCorrect = false;*/
                    if (eMsg.status.Length > 100)
                        isCorrect = false;
                    if (eMsg.avatarId > MaxAvatar || eMsg.avatarId < 0)
                        isCorrect = false;
                    if (accountManager.UsernameExists(eMsg.name, userAccount.Id))
                    {
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.EditProfile,
                            TextError = "This user exist"
                        });
                        break;
                    }
                    if (isCorrect)
                    {
                        userAccount.Name = eMsg.name;
                        userAccount.Status = eMsg.status;
                        userAccount.AvatarId = eMsg.avatarId;

                        client.Player.Name = eMsg.name;
                        client.Player.AvatarId = eMsg.avatarId;
                        client.Player.Status = eMsg.status;
                        
                        await client.SendAsync(new ConfirmMessage(
                            RequestType.EditProfile,
                            ""));

                        Console.WriteLine($"Account data changed {client.Player.Id}");
                    }
                    else
                    {
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.EditProfile,
                            TextError = "Incorrect data"
                        });
                    }
                    break;
                case ConnectMessage:
                    try
                    {
                        var room = JoinRoom(msg, client);
                        room.Engine.players.Add(client.Player);
                        client.Room = room;
                        client.Player.IsHost = false;

                        var okMsg1 = new ConfirmMessage(RequestType.ConnectRoom, "ok");
                        await client.SendAsync(okMsg1);


                        var newPlayer = new PlayerInfo(client.Player.Id, client.Player.Name, -1,client.Player.IsHost, client.Player.Status, client.Player.AvatarId, client.Player.Wins, client.Player.CountGames, client.Player.Points);
                        
                        foreach (var other in room.Clients) // отправляем остальным, что кто-то присоединился
                        {
                            if (other.Player.Id != client.Player.Id)
                            {
                                Console.WriteLine($"Notify {other.Player.Name} about new player {client.Player.Name}");
                                
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
                                var player = new PlayerInfo(other.Player.Id, other.Player.Name, -1, other.Player.IsHost, other.Player.Status, other.Player.AvatarId, other.Player.Wins, other.Player.CountGames, other.Player.Points);
                                
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
