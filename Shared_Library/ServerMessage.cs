using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Library
{
    public abstract class ServerMessage
    {
        public string Type { get; set; }
    }

    public class ConnectMessage : ServerMessage
    {
        public string CodeRoom { get; set; }
    }

    public class CreateRoomMessage : ServerMessage
    {

    }

    public class TakeCardMessage : ServerMessage
    {
        
    }

    public class PlayCardMessage : ServerMessage
    {
        public Card card { get; set; }
        public ColorCard chosenColor { get; set; }
    }

    public class StartGameMessage : ServerMessage
    {
        
    }

    public class ErrorMessage : ServerMessage
    {
        public string TextError { get; set; }
    }

    public class PushUNOMessage : ServerMessage
    {
        public int indexPush {  get; set; } // кому нажали
    }
    public class SignUpMessage : ServerMessage
    {
        public string name {  get; set; }
        public string password { get; set; }
    }
    public class LoginMessage : ServerMessage
    {
        public string name { get; set; }
        public string password { get; set; }
    }
    public class GameStateMessage : ServerMessage
    {
        public List<PlayerInfo> players;
        public List<Card> MyCards;
        public int currentPlayer;
        public int direction;
        public Card lastCard;
        public ColorCard currentColor;
        public bool IsMyTurn;

        public GameStateMessage(List<PlayerInfo> players, int currentPlayer, int direction,
            Card lastCard, ColorCard currentColor, bool isMyTurn)
        {
            this.players = players;
            this.currentPlayer = currentPlayer;
            this.direction = direction;
            this.lastCard = lastCard;
            this.currentColor = currentColor;
            this.IsMyTurn = isMyTurn;
        }
    }
}
