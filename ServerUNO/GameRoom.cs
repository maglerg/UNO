using Shared_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Server
{
    public class GameRoom
    {
        public List<ClientConnection> Clients = new List<ClientConnection>();
        public GameEngine Engine;

        /*class GameRoom
        {
            public List<Player> Clients.Player;
        }*/
        public async Task SendGameStage()
        {

            List<PlayerInfo> infos = new List<PlayerInfo>();
            for (int i = 0; i < Clients.Count; i++)
            {
                var playerinf = new PlayerInfo(Clients[i].Player.Name, Clients[i].Player.PlayerCards.Count);
                infos.Add(playerinf);
            }
            for (int i = 0; i < Clients.Count; i++)
            {
                bool isMyTurn = false;
                if (i == Engine.currentPlayer)
                    isMyTurn = true;
                    var msgdto = new GameStateMessage(infos, Engine.currentPlayer,
                    Engine.direction, Engine.lastCard, Engine.currentColor, isMyTurn);
                await Clients[i].SendAsync(msgdto);
            }
            
        }
        public async Task ProcessMessage(Player player, ServerMessage msg)
        {
            int indexPlayer = -1;
            for (int i = 1; i < Engine.players.Count; i++)
            {
                Player temp = Engine.players[i];
                if (temp == player)
                {
                    indexPlayer = i;
                    break;
                }
            }
            if (indexPlayer == -1)
                throw new ArgumentException("Игрок не найден");

            switch (msg)
            {
                case StartGameMessage:
                    if (CheckStartGame())
                    {
                        Engine.StartGame();

                        await SendGameStage();
                    }
                    else
                    {
                        foreach (var client in Clients)
                        {
                            await client.SendAsync(new ErrorMessage
                            {
                                TextError = "Нельзя начать игру"
                            });
                        }
                    }   
                    break;

                case TakeCardMessage:
                    Engine.TakeCard(indexPlayer);
                    await SendGameStage();
                    break;

                case PlayCardMessage playMsg:
                    Engine.PlayCard(indexPlayer, playMsg.card, playMsg.chosenColor);
                    await SendGameStage();
                    break;
                case PushUNOMessage pushunoMsg:
                    Engine.PushUno(pushunoMsg.indexPush);
                    await SendGameStage();
                    break;
                default:
                    foreach (var client in Clients)
                    {
                        await client.SendAsync(new ErrorMessage
                        {
                            TextError = "Несуществующее действие"
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


