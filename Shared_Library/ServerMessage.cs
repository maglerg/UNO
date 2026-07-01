using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Library
{
    public enum RequestType
    {
        Login,
        SignUp,
        CreateRoom,
        ConnectRoom,
        SomeoneJoin,
        SomeoneLeft,
        StartGame,
        LeaveGame,
        GameState,
        
        TakeCard,
        PlayCard,
        PlayUno
    }
    public abstract class ServerMessage
    {
        public string Type { get; set; }
    }

    public class ConnectMessage : ServerMessage
    {
        public string CodeRoom { get; set; }
        public ConnectMessage(string codeRoom)
        {
            this.CodeRoom = codeRoom;
        }
    }
    public class LeaveGameMessage : ServerMessage
    {

    }
    public class SomeoneJoin : ServerMessage
    {
        public PlayerInfo Player { get; set; }
        public SomeoneJoin(PlayerInfo player)
        {
            this.Player = player;
        }
    }
    public class SomeoneLeft : ServerMessage
    {
        public PlayerInfo Player { get; set; }
        public SomeoneLeft(PlayerInfo player)
        {
            Player = player;
        }
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
        public ColorCard? chosenColor { get; set; }
        public PlayCardMessage(Card card, ColorCard? chosenColor = null)
        {
            this.card = card;
            this.chosenColor = chosenColor;
        }
    }

    public class StartGameMessage : ServerMessage
    {
        
    }

    public class ErrorMessage : ServerMessage
    {
        public RequestType RequestType { get; set; }
        public string TextError { get; set; }
    }
    public class ConfirmMessage : ServerMessage
    {
        public RequestType RequestType { get; set; }
        public string roomCode { get; set; }
        public ConfirmMessage(RequestType RequestType,string roomCode)
        {
            this.RequestType = RequestType;
            this.roomCode = roomCode;
        }
    }

    public class PushUNOMessage : ServerMessage
    {
        public int indexPush {  get; set; } // кому нажали
        public PushUNOMessage(int indexPush)
        {
            this.indexPush = indexPush;
        }
    }
    public class SignUpMessage : ServerMessage
    {
        public string name {  get; set; }
        public string password { get; set; }
        public SignUpMessage(string name, string password)
        {
            this.name = name;
            this.password = password;
        }
    }
    public class LoginMessage : ServerMessage
    {
        public string name { get; set; }
        public string password { get; set; }
        public LoginMessage(string name, string password)
        {
            this.name = name;
            this.password = password;
        }
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
