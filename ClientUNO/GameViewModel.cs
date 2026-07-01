using Microsoft.Win32;
using Shared_Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientUNO
{
    public class GameViewModel
    {
        public GameState state { get; set; }

        public event Action<ConfirmMessage> StartGame;
        public event Action<ErrorMessage> StartGameError;
        public event Action<SomeoneJoin> SomeoneJoin;
        public event Action<SomeoneLeft> SomeoneLeft;
        public event Action? GameStateChanged;

        private TaskCompletionSource<ConfirmMessage>? _createRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _connectRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _leaveRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _signUpTcs;
        private TaskCompletionSource<ConfirmMessage>? _loginTcs;
        private TaskCompletionSource<ConfirmMessage>? _takeCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playUnoTcs;

        public GameViewModel()
        {
            state = new GameState();
        }
        public void ProcessMessage(ServerMessage msg)
        {
            switch (msg)
            {
                case ConfirmMessage confirm:
                    ProcessConfirm(confirm);
                    break;

                case ErrorMessage error:
                    ProcessError(error);
                    break;
                case SomeoneJoin someoneJ:
                    SomeoneJoin?.Invoke(someoneJ);
                    break;
                case SomeoneLeft someoneL:
                    SomeoneLeft?.Invoke(someoneL);
                    break;
                case GameStateMessage game:
                    state.Update(game);
                    GameStateChanged?.Invoke();
                    break;

                default:
                    throw new ArgumentException("Unknown message type"); 
            }
        }

        private void ProcessConfirm(ConfirmMessage confirm)
        {
           
            switch (confirm.RequestType)
            {
                case RequestType.Login:
                    _loginTcs?.SetResult(confirm);
                    _loginTcs = null;
                    break;

                case RequestType.SignUp:
                    _signUpTcs?.SetResult(confirm);
                    _signUpTcs = null;
                    break;
                case RequestType.CreateRoom:
                    _createRoomTcs?.SetResult(confirm);
                    _createRoomTcs = null;
                    break;
                case RequestType.ConnectRoom:
                    _connectRoomTcs?.SetResult(confirm);
                    _connectRoomTcs = null;
                    break;
                case RequestType.StartGame:
                    StartGame?.Invoke(confirm);
                    break;
                case RequestType.LeaveGame:
                    _leaveRoomTcs?.SetResult(confirm);
                    _leaveRoomTcs = null;
                    break;
                case RequestType.GameState:
                    
                    break;
                case RequestType.TakeCard:
                    _takeCardTcs?.SetResult(confirm);
                    _takeCardTcs = null;
                    break;
                case RequestType.PlayCard:
                    _playCardTcs?.SetResult(confirm);
                    _playCardTcs = null;
                    break;
                case RequestType.PlayUno:
                    _playUnoTcs?.SetResult(confirm);
                    _playUnoTcs = null;
                    break;
            }
        }

        private void ProcessError(ErrorMessage error)
        {
            switch (error.RequestType)
            {
                case RequestType.Login:
                    _loginTcs?.SetException(new Exception(error.TextError));
                    _loginTcs = null;
                    break;

                case RequestType.SignUp:
                    _signUpTcs?.SetException(new Exception(error.TextError));
                    _signUpTcs = null;
                    break;
                case RequestType.CreateRoom:
                    _createRoomTcs?.SetException(new Exception(error.TextError));
                    _createRoomTcs = null;
                    break;
                case RequestType.ConnectRoom:
                    _connectRoomTcs?.SetException(new Exception(error.TextError));
                    _connectRoomTcs = null;
                    break;
                case RequestType.StartGame:
                    StartGameError?.Invoke(error);
                    break;
                case RequestType.LeaveGame:
                    _leaveRoomTcs?.SetException(new Exception(error.TextError));
                    _leaveRoomTcs = null;
                    break;
                case RequestType.GameState:
                    break;
                case RequestType.TakeCard:
                    _takeCardTcs?.SetException(new Exception(error.TextError));
                    _takeCardTcs = null;
                    break;
                case RequestType.PlayCard:
                    _playCardTcs?.SetException(new Exception(error.TextError));
                    _playCardTcs = null;
                    break;
                case RequestType.PlayUno:
                    _playUnoTcs?.SetException(new Exception(error.TextError));
                    _playUnoTcs = null;
                    break;
            }
        }
        public async Task<ConfirmMessage> CreateRoomAsync()
        {
            _createRoomTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new CreateRoomMessage());

            return await _createRoomTcs.Task;
        }
        public async Task<ConfirmMessage> ConnectRoomAsync(string code)
        {
            _connectRoomTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new ConnectMessage(code));

            return await _connectRoomTcs.Task;
        }
        
        public async Task<ConfirmMessage> LeftRoomAsync()
        {
            _leaveRoomTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new LeaveGameMessage());

            return await _leaveRoomTcs.Task;
        }
        public async Task<ConfirmMessage> SignUpAsync(string name, string password)
        {
            _signUpTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new SignUpMessage(name, password));

            return await _signUpTcs.Task;
        }
        public async Task<ConfirmMessage> LoginAsync(string name, string password)
        {
            _loginTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new SignUpMessage(name, password));

            return await _loginTcs.Task;
        }
        public async Task<ConfirmMessage> TakeCardAsync()
        {
            _takeCardTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new TakeCardMessage());

            return await _takeCardTcs.Task;
        }
        public async Task<ConfirmMessage> PlayCardAsync(Card card, ColorCard? chosenColor)
        {
            _playCardTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new PlayCardMessage(card, chosenColor));

            return await _playCardTcs.Task;
        }
        public async Task<ConfirmMessage> PlayUnoAsync(int indexPush)
        {
            _playUnoTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new PushUNOMessage(indexPush));

            return await _playUnoTcs.Task;
        }
    }
}
