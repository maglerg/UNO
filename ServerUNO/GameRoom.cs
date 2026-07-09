using Shared_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Server
{
    public class GameRoom
    {
        public List<ClientConnection> Clients = new List<ClientConnection>();
        public GameEngine Engine;
        public Guid HostId { get; set; }
        public GameRoom()
        {
            Engine = new GameEngine();
            for(int i = 0; i < Clients.Count; i++) 
            {
                Engine.players.Add(Clients[i].Player);
            }
        }

        public async Task SendGameStage()
        {

            List<PlayerInfo> infos = new List<PlayerInfo>();
            for (int i = 0; i < Clients.Count; i++)
            {
                var playerinf = new PlayerInfo(Clients[i].Player.Id, Clients[i].Player.Name, Clients[i].Player.PlayerCards.Count, Clients[i].Player.IsHost);
                infos.Add(playerinf);
            }
            for (int i = 0; i < Clients.Count; i++)
            {
                bool isMyTurn = false;
                if (i == Engine.currentPlayer)
                    isMyTurn = true;
                    var msgdto = new GameStateMessage(infos, Clients[i].Player.PlayerCards, Engine.currentPlayer,
                    Engine.direction, Engine.lastCard, Engine.currentColor, isMyTurn);
                await Clients[i].SendAsync(msgdto);
            }
            
        }
        public async Task ProcessMessage(Player player, ServerMessage msg)
        {
            int indexPlayer = -1;
            for (int i = 0; i < Engine.players.Count; i++)
            {
                Player temp = Engine.players[i];
                if (temp == player)
                {
                    indexPlayer = i;
                    break;
                }
            }
            if (indexPlayer == -1)
                throw new ArgumentException("Player was not found");

            switch (msg)
            {
                case StartGameMessage:
                    if (CheckStartGame())
                    {
                        Engine.StartGame();
                        // отправиль подтверждение игры
                        await SendGameStage();
                    }
                    else
                    {
                        foreach (var client in Clients)
                        {
                            await client.SendAsync(new ErrorMessage
                            {
                                RequestType = RequestType.StartGame,
                                TextError = "Can't start the game"
                            });
                        }
                    }   
                    break;
                case LeaveGameMessage:
                    try
                    {
                        var leavingClient = Clients[indexPlayer];

                        Clients.Remove(leavingClient);
                        Engine.players.Remove(player);
                        leavingClient.Room = null;

                        var okMsg4 = new ConfirmMessage(RequestType.LeaveGame, "ok");
                        await leavingClient.SendAsync(okMsg4);
                        Console.WriteLine("SEND LEAVE CONFIRM");

                        for (int i = 0; i < Clients.Count; i++)
                        {
                            var client = Clients[i];

                            var person = new PlayerInfo(
                                leavingClient.Player.Id,
                                leavingClient.Player.Name,
                                -1, leavingClient.Player.IsHost);

                            await client.SendAsync(new SomeoneLeftMessage(person));
                        }
                        if (Clients.Count > 0 )
                        {
                            Clients[0].Player.IsHost = true;
                            Clients[0].SendAsync(new ChangeHostMessage());// отправить нового хоста
                        }
                        
                        
                    }
                    catch (Exception ex)
                    {
                        foreach (var client in Clients)
                        {
                            await client.SendAsync(new ErrorMessage
                            {
                                RequestType = RequestType.LeaveGame,
                                TextError = $"Couldn't leave game. {ex.Message}"
                            });
                        }
                    }
                    break;
                case TakeCardMessage:
                    Engine.TakeCard(indexPlayer);
                    var okMsg1 = new ConfirmMessage(RequestType.TakeCard, "ok");
                    await Clients[indexPlayer].SendAsync(okMsg1);
                    await SendGameStage();
                    break;

                case PlayCardMessage playMsg:
                    Engine.PlayCard(indexPlayer, playMsg.card, playMsg.chosenColor);
                    var okMsg2 = new ConfirmMessage(RequestType.PlayCard, "ok");
                    await Clients[indexPlayer].SendAsync(okMsg2);
                    await SendGameStage();
                    break;
                case PushUNOMessage pushunoMsg:
                    Engine.PushUno(pushunoMsg.indexPush);
                    var okMsg3 = new ConfirmMessage(RequestType.PlayUno, "ok");
                    await Clients[indexPlayer].SendAsync(okMsg3);
                    await SendGameStage();
                    break;
                default:
                    foreach (var client in Clients)
                    {
                        await client.SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.GameState,
                            TextError = "Non-existent action"
                        });
                    }
                    break;
            }
        }
        public bool CanJoinPlayer(Player person)
        {
            if (Clients.Count < 4 && Clients.Count >= 0)
                return true;
            
            else return false;
        }
        public bool CheckStartGame()
        {
            if (Clients.Count < 2 || Clients.Count > 4)
                return false;
            return true;
        }
    }
}


