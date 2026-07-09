
using Shared_Library;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ClientUNO
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public GameState state { get; set; }

        public event Action<ConfirmMessage> StartGame;
        public event Action<ErrorMessage> StartGameError;
        public event Action<SomeoneJoinMessage> SomeoneJoin;
        public event Action<SomeoneLeftMessage> SomeoneLeft;
        public event Action ChangeHost;

        private TaskCompletionSource<ConfirmMessage>? _createRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _connectRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _leaveRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _signUpTcs;
        private TaskCompletionSource<ConfirmMessage>? _loginTcs;
        private TaskCompletionSource<ConfirmMessage>? _takeCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playUnoTcs;

        public ICommand DrawCardCommand { get; }
        public ICommand PlayCardCommand { get; }
        public ICommand UnoCommand { get; }

        public List<Card> Cards => state.MyCards;
        public Card TopCard => state.lastCard;
        public int MyCardsCount => state.MyCards.Count;
        public bool CanSayUno => state.MyCards.Count == 1;

        public string ErrorMessage { get; set; }
        public Visibility ErrorVisibility { get; set; } = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Card? _selectedCard;

        private bool _isColorSelectionVisible;

        public bool IsColorSelectionVisible
        {
            get => _isColorSelectionVisible;
            set
            {
                _isColorSelectionVisible = value;
                OnPropertyChanged(nameof(IsColorSelectionVisible));
                OnPropertyChanged(nameof(ColorSelectionVisibility));
            }
        }

        public Visibility ColorSelectionVisibility =>
            IsColorSelectionVisible ? Visibility.Visible : Visibility.Collapsed;

        public ICommand ChooseColorCommand { get; }

        public PlayerInfo TopPlayer => state.Players[0];
        public PlayerInfo LeftPlayer => state.Players[1];
        public PlayerInfo RightPlayer => state.Players[2];
        public PlayerInfo Me => state.Players[3];

        public Visibility TopPlayerUnoVisibility =>
    TopPlayer.CardCount == 1 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility LeftPlayerUnoVisibility =>
            LeftPlayer.CardCount == 1 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility RightPlayerUnoVisibility =>
            RightPlayer.CardCount == 1 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility MyUnoVisibility =>
            Me.CardCount == 1 ? Visibility.Visible : Visibility.Collapsed;


        public GameViewModel()
        {
            state = new GameState();

            DrawCardCommand = new RelayCommand(() => _ =  TakeCardAsync());
            PlayCardCommand = new RelayCommand<Card>(card => _ = PlayCardCommandExecute(card));
            UnoCommand = new RelayCommand<int>(index => _ = PlayUnoAsync(index));
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
                case SomeoneJoinMessage someoneJ:
                    SomeoneJoin?.Invoke(someoneJ);
                    break;
                case SomeoneLeftMessage someoneL:
                    SomeoneLeft?.Invoke(someoneL);
                    break;
                case ChangeHostMessage:
                    ChangeHost?.Invoke();
                    break;
                    
                case GameStateMessage game:
                    state.Update(game);
                    OnPropertyChanged(nameof(Cards));
                    OnPropertyChanged(nameof(TopCard));
                    OnPropertyChanged(nameof(MyCardsCount));
                    OnPropertyChanged(nameof(CanSayUno));

                    OnPropertyChanged(nameof(TopPlayer));
                    OnPropertyChanged(nameof(LeftPlayer));
                    OnPropertyChanged(nameof(RightPlayer));
                    OnPropertyChanged(nameof(Me));

                    OnPropertyChanged(nameof(TopPlayerUnoVisibility));
                    OnPropertyChanged(nameof(LeftPlayerUnoVisibility));
                    OnPropertyChanged(nameof(RightPlayerUnoVisibility));
                    OnPropertyChanged(nameof(MyUnoVisibility));
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

            await App.Session.Client.SendAsync(new LoginMessage(name, password));

            return await _loginTcs.Task;
        }
        public async Task<ConfirmMessage> TakeCardAsync()
        {
            _takeCardTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new TakeCardMessage());

            return await _takeCardTcs.Task;
        }
        private async Task PlayCardCommandExecute(Card card)
        {
            if (card.Color == ColorCard.Black)
            {
                _selectedCard = card;
                IsColorSelectionVisible = true;
                return;
            }

            await PlayCardAsync(card, null);
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
        public void ShowError(string message)
        {
            ErrorMessage = message;
            ErrorVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(ErrorVisibility));

            Task.Delay(2000).ContinueWith(_ =>
            {
                ErrorVisibility = Visibility.Collapsed;
                OnPropertyChanged(nameof(ErrorVisibility));
            });
        }
    }
}
