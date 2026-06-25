using Server;
using Shared_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUNO
{
    public class RoomManager
    {
        Dictionary<string, GameRoom> Rooms = new();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public async Task ProcessMessage(ClientConnection client, ServerMessage msg)
        {
            switch (msg)
            {
                case ConnectMessage:
                    try
                    {
                        JoinRoom(msg, client);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case CreateRoomMessage:
                    CreateRoom();
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
        public void CreateRoom()
        {
            GameRoom newRoom = new GameRoom();

            string code;
            do
            {
                code = GenerateCode();
            }
            while (Rooms.ContainsKey(code));

            Rooms.Add(code, newRoom);
        }
        public GameRoom GetRoom(string code)
        {
            Rooms.TryGetValue(code, out GameRoom room);
            return room;
        }
        public void JoinRoom(ServerMessage message, ClientConnection client)
        {
            var connectMsg = message as ConnectMessage;

            if (connectMsg == null)
                return;

            string code = connectMsg.CodeRoom;

            GameRoom room = GetRoom(code);

            if (room.CanJoinPlayer(client.Player))
            {
                room.Clients.Add(client);
            }
        }
        // удалить комнату
    }
}
