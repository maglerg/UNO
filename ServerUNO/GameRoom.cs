using Shared_Library;

namespace Server
{
    public class GameRoom
    {
        public List<ClientConnection> Clients = new List<ClientConnection>();
        public GameEngine Engine;
        public Guid HostId { get; set; }
        bool IsGameStarted { get; set; }
        public GameRoom()
        {
            Engine = new GameEngine();
            for(int i = 0; i < Clients.Count; i++) 
            {
                Engine.players.Add(Clients[i].Player);
            }
            IsGameStarted = false;
        }

        public async Task SendGameStage()
        {
            for (int k = 0; k < Clients.Count; k++)
            {
                List<PlayerInfo> infos = new List<PlayerInfo>();
                int currPl = -1;


                // добавляем себя первым
                if (k == Engine.currentPlayer)
                    currPl = infos.Count;

                infos.Add(new PlayerInfo(
                    Clients[k].Player.Id,
                    Clients[k].Player.Name,
                    Clients[k].Player.PlayerCards.Count,
                    Clients[k].Player.IsHost,
                    Clients[k].Player.Status,
                    Clients[k].Player.AvatarId
                    ));


                // игроки после k
                for (int i = k + 1; i < Clients.Count; i++)
                {
                    if (i == Engine.currentPlayer)
                        currPl = infos.Count;

                    infos.Add(new PlayerInfo(
                        Clients[i].Player.Id,
                        Clients[i].Player.Name,
                        Clients[i].Player.PlayerCards.Count,
                        Clients[i].Player.IsHost,
                        Clients[i].Player.Status,
                        Clients[i].Player.AvatarId
                    ));
                }


                // игроки до k
                for (int i = 0; i < k; i++)
                {
                    if (i == Engine.currentPlayer)
                        currPl = infos.Count;

                    infos.Add(new PlayerInfo(
                        Clients[i].Player.Id,
                        Clients[i].Player.Name,
                        Clients[i].Player.PlayerCards.Count,
                        Clients[i].Player.IsHost,
                        Clients[i].Player.Status,
                        Clients[i].Player.AvatarId
                    ));
                }


                bool isMyTurn = k == Engine.currentPlayer;


                var msgdto = new GameStateMessage(
                    infos,
                    Clients[k].Player.PlayerCards,
                    currPl,
                    Engine.direction,
                    Engine.lastCard,
                    Engine.currentColor,
                    isMyTurn);


                await Clients[k].SendAsync(msgdto);
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
                case StartGameMessageToServer:
                    try
                    {
                        Engine.StartGame();
                        Console.WriteLine("Start game.");
                        IsGameStarted = true;

                        for (int k = 0; k < Clients.Count; k++)
                        {
                            List<PlayerInfo> infos = new List<PlayerInfo>();
                            int currPl = -1;


                            // добавляем себя первым
                            if (k == Engine.currentPlayer)
                                currPl = infos.Count;

                            infos.Add(new PlayerInfo(
                                Clients[k].Player.Id,
                                Clients[k].Player.Name,
                                Clients[k].Player.PlayerCards.Count,
                                Clients[k].Player.IsHost,
                                Clients[k].Player.Status,
                                Clients[k].Player.AvatarId
                            ));


                            // игроки после k
                            for (int i = k + 1; i < Clients.Count; i++)
                            {
                                if (i == Engine.currentPlayer)
                                    currPl = infos.Count;

                                infos.Add(new PlayerInfo(
                                    Clients[i].Player.Id,
                                    Clients[i].Player.Name,
                                    Clients[i].Player.PlayerCards.Count,
                                    Clients[i].Player.IsHost,
                                    Clients[i].Player.Status,
                                    Clients[i].Player.AvatarId
                                ));
                            }


                            // игроки до k
                            for (int i = 0; i < k; i++)
                            {
                                if (i == Engine.currentPlayer)
                                    currPl = infos.Count;

                                infos.Add(new PlayerInfo(
                                    Clients[i].Player.Id,
                                    Clients[i].Player.Name,
                                    Clients[i].Player.PlayerCards.Count,
                                    Clients[i].Player.IsHost,
                                    Clients[i].Player.Status,
                                    Clients[i].Player.AvatarId
                                    ));
                            }


                            bool isMyTurn = k == Engine.currentPlayer;


                            var msgdto = new GameStateMessage(
                                infos,
                                Clients[k].Player.PlayerCards,
                                currPl,
                                Engine.direction,
                                Engine.lastCard,
                                Engine.currentColor,
                                isMyTurn);


                            await Clients[k].SendAsync(new StartGameMessageToClient(msgdto));
                        }
                    } 
                    catch(Exception ex)
                    {
                        await Clients[indexPlayer].SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.StartGame,
                            TextError = ex.Message
                        });
                    }
                    break;
                case CanStartGameMessage:
                    if (CheckStartGame())
                    {
                        if(Clients[indexPlayer].Player.Id == HostId)
                        {
                            if (!IsGameStarted)
                            {
                                await Clients[indexPlayer].SendAsync(new ConfirmMessage(RequestType.StartGame, "ok"));
                            }
                            else
                            {
                                await Clients[indexPlayer].SendAsync(new ErrorMessage
                                {
                                    RequestType = RequestType.StartGame,
                                    TextError = "Game already started."
                                });
                            }
                        }
                        else
                        {
                            await Clients[indexPlayer].SendAsync(new ErrorMessage
                            {
                                RequestType = RequestType.StartGame,
                                TextError = "This player isn't host."
                            });
                        }
                    }
                    else
                    {
                        await Clients[indexPlayer].SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.StartGame,
                            TextError = "Not enough players to start the game."
                        });
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
                                -1, leavingClient.Player.IsHost,
                                leavingClient.Player.Status,
                                leavingClient.Player.AvatarId);

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
                    try
                    {
                        Engine.TakeCard(indexPlayer);
                        var okMsg1 = new ConfirmMessage(RequestType.TakeCard, "ok");
                        await Clients[indexPlayer].SendAsync(okMsg1);
                        await SendGameStage();
                        break;
                    }
                    catch(Exception ex)
                    {
                        await Clients[indexPlayer].SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.GameState,
                            TextError = $"{ex.Message}"
                        });
                        break;
                    }
                   

                case PlayCardMessage playMsg:
                    try
                    {
                        Engine.PlayCard(indexPlayer, playMsg.card, playMsg.chosenColor);
                        var okMsg2 = new ConfirmMessage(RequestType.PlayCard, "ok");
                        await Clients[indexPlayer].SendAsync(okMsg2);
                        await SendGameStage();
                        break;
                    }
                    catch (Exception ex)
                    {
                        await Clients[indexPlayer].SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.GameState,
                            TextError = $"{ex.Message}"
                        });
                        break;
                    }
                    
                case PushUNOMessage pushunoMsg:
                    try
                    {
                        Engine.PushUno(pushunoMsg.indexPush);
                        var okMsg3 = new ConfirmMessage(RequestType.PlayUno, "ok");
                        await Clients[indexPlayer].SendAsync(okMsg3);
                        await SendGameStage();
                        break;
                    }
                    catch (Exception ex)
                    {
                        await Clients[indexPlayer].SendAsync(new ErrorMessage
                        {
                            RequestType = RequestType.GameState,
                            TextError = $"{ex.Message}"
                        });
                        break;
                    }
                    
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


