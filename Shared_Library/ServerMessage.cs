using System;
using System.Collections.Generic;

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

        protected ServerMessage(string type)
        {
            Type = type;
        }
    }


    public class LoginMessage : ServerMessage
    {
        public string name { get; set; }
        public string password { get; set; }

        public LoginMessage(string name, string password)
            : base(nameof(LoginMessage))
        {
            this.name = name;
            this.password = password;
        }
    }

    public class SignUpMessage : ServerMessage
    {
        public string name { get; set; }
        public string password { get; set; }

        public SignUpMessage(string name, string password)
            : base(nameof(SignUpMessage))
        {
            this.name = name;
            this.password = password;
        }
    }

    public class CreateRoomMessage : ServerMessage
    {
        public CreateRoomMessage()
            : base(nameof(CreateRoomMessage))
        {
        }
    }

    public class ConnectMessage : ServerMessage
    {
        public string CodeRoom { get; set; }

        public ConnectMessage(string codeRoom)
            : base(nameof(ConnectMessage))
        {
            CodeRoom = codeRoom;
        }
    }

    public class LeaveGameMessage : ServerMessage
    {
        public LeaveGameMessage()
            : base(nameof(LeaveGameMessage))
        {
        }
    }


    public class TakeCardMessage : ServerMessage
    {
        public TakeCardMessage()
            : base(nameof(TakeCardMessage))
        {
        }
    }

    public class PlayCardMessage : ServerMessage
    {
        public Card card { get; set; }
        public ColorCard? chosenColor { get; set; }

        public PlayCardMessage(Card card, ColorCard? chosenColor = null)
            : base(nameof(PlayCardMessage))
        {
            this.card = card;
            this.chosenColor = chosenColor;
        }
    }

    public class PushUNOMessage : ServerMessage
    {
        public int indexPush { get; set; }

        public PushUNOMessage(int indexPush)
            : base(nameof(PushUNOMessage))
        {
            this.indexPush = indexPush;
        }
    }

    public class SomeoneJoin : ServerMessage
    {
        public PlayerInfo Player { get; set; }

        public SomeoneJoin(PlayerInfo player)
            : base(nameof(SomeoneJoin))
        {
            Player = player;
        }
    }

    public class SomeoneLeft : ServerMessage
    {
        public PlayerInfo Player { get; set; }

        public SomeoneLeft(PlayerInfo player)
            : base(nameof(SomeoneLeft))
        {
            Player = player;
        }
    }

    public class StartGameMessage : ServerMessage
    {
        public StartGameMessage()
            : base(nameof(StartGameMessage))
        {
        }
    }


    public class ErrorMessage : ServerMessage
    {
        public RequestType RequestType { get; set; }
        public string TextError { get; set; }

        public ErrorMessage(RequestType requestType, string textError)
            : base(nameof(ErrorMessage))
        {
            RequestType = requestType;
            TextError = textError;
        }
    }

    public class ConfirmMessage : ServerMessage
    {
        public RequestType RequestType { get; set; }
        public string roomCode { get; set; }

        public ConfirmMessage(RequestType requestType, string roomCode)
            : base(nameof(ConfirmMessage))
        {
            RequestType = requestType;
            this.roomCode = roomCode;
        }
    }

    public class GameStateMessage : ServerMessage
    {
        public List<PlayerInfo> players { get; set; }
        public List<Card> MyCards { get; set; }
        public int currentPlayer { get; set; }
        public int direction { get; set; }
        public Card lastCard { get; set; }
        public ColorCard currentColor { get; set; }
        public bool IsMyTurn { get; set; }

        public GameStateMessage(List<PlayerInfo> players, List<Card> MyCards, int currentPlayer, int direction, Card lastCard, ColorCard currentColor, bool IsMyTurn)
            : base(nameof(GameStateMessage))
        {
            this.players = players;
            this.MyCards = MyCards;
            this.currentPlayer = currentPlayer;
            this.direction = direction;
            this.lastCard = lastCard;
            this.currentColor = currentColor;
            this.IsMyTurn = IsMyTurn;
        }
    }
}