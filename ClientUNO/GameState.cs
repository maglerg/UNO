using Shared_Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientUNO
{
    public class GameState
    {
        public List<PlayerInfo> Players { get; set; }
        public List<Card> MyCards { get; set; }
        public int currentPlayer { get; set; }
        public int direction;
        public Card lastCard;
        public ColorCard currentColor;
        public bool IsMyTurn {  get; set; }

        public void Update(GameStateMessage msg)
        {
            Players = msg.players;
            MyCards = msg.MyCards;
            currentPlayer= msg.currentPlayer;
            this.direction = msg.direction;
            this.lastCard = msg.lastCard;
            this.currentColor = msg.currentColor;
            IsMyTurn = msg.IsMyTurn;
        }
    }
}
