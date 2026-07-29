using System.Windows.Media;
using Shared_Library;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ClientUNO
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public int avatarcount = 6;
        private bool _startMessageShown;
        public GameState state { get; set; }

        public event Action<ConfirmMessage> CanStartGame;
        public event Action<ErrorMessage> StartGameError;
        public event Action<SomeoneJoinMessage> SomeoneJoin;
        public event Action<SomeoneLeftMessage> SomeoneLeft;
        public event Action ChangeHost;
        public event Action<StartGameMessageToClient> StartGameToClient;

        private TaskCompletionSource<ConfirmMessage>? _createRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _connectRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _leaveRoomTcs;
        private TaskCompletionSource<ConfirmMessage>? _signUpTcs;
        private TaskCompletionSource<ConfirmMessage>? _editProfileTcs; 
        private TaskCompletionSource<ConfirmMessage>? _loginTcs;
        private TaskCompletionSource<ConfirmMessage>? _takeCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playCardTcs;
        private TaskCompletionSource<ConfirmMessage>? _playUnoTcs;

        public ICommand DrawCardCommand { get; }
        public ICommand PlayCardCommand { get; }
        public ICommand UnoCommand { get; }

        public List<Card> Cards => state.MyCards;
        public Card TopCard => state.lastCard;
        private string _notificationText;

        public string NotificationText
        {
            get => _notificationText;
            set
            {
                _notificationText = value;
                OnPropertyChanged(nameof(NotificationText));
            }
        }


        private Visibility _notificationVisibility = Visibility.Collapsed;

        public Visibility NotificationVisibility
        {
            get => _notificationVisibility;
            set
            {
                _notificationVisibility = value;
                OnPropertyChanged(nameof(NotificationVisibility));
            }
        }
        private async Task ShowNotification(string text)
        {
            NotificationText = text;

            NotificationVisibility = Visibility.Visible;


            await Task.Delay(2000);


            NotificationVisibility = Visibility.Collapsed;
        }
        private string GetCardNotification(Card card)
        {
            int nextPlayer =
                (state.currentPlayer + state.direction + state.Players.Count)
                % state.Players.Count;


            PlayerInfo player = state.Players[nextPlayer];

            switch (card.Type)
            {
                case TypeCard.Skip:
                    return $"{player.Name} must skip a turn";

                case TypeCard.DrawTwo:
                    return $"{player.Name} must draw 2 cards";

                case TypeCard.Wild:
                    return $"Color selected: {card.Color}";

                case TypeCard.WildDrawFour:
                    return $"{player.Name} must draw 4 cards. Color selected: {card.Color}";

                case TypeCard.Reverse:
                    return "Game direction has changed";

                case TypeCard.Number:
                    return null;

                default:
                    return null;
            }
        }
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
        public string SelectedColorText
        {
            get
            {
                if (TopCard == null || TopCard.Color != ColorCard.Black)
                    return "";

                return $"The color is selected: {state.currentColor}";
            }
        }

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

        public PlayerInfo? MePlayer => state.Players.Count > 0 ? state.Players[0] : null;
        public PlayerInfo? LeftPlayer => state.Players.Count > 1 ? state.Players[1] : null;
        public PlayerInfo? TopPlayer => state.Players.Count > 2 ? state.Players[2] : null;
        public PlayerInfo? RightPlayer => state.Players.Count > 3 ? state.Players[3] : null;
        public Visibility LeftPlayerVisibility => // видимость игроков
    LeftPlayer == null
        ? Visibility.Collapsed
        : Visibility.Visible;
        public Visibility TopPlayerVisibility =>
    TopPlayer == null
        ? Visibility.Collapsed
        : Visibility.Visible;
        public Visibility RightPlayerVisibility =>
    RightPlayer == null
        ? Visibility.Collapsed
        : Visibility.Visible;

        public Visibility RightPlayerUnoVisibility => // видимость кнопки уно
    RightPlayer != null && RightPlayer.CardCount == 1 &&
    !RightPlayer.UnoPressed
        ? Visibility.Visible
        : Visibility.Collapsed;
       
        public Visibility TopPlayerUnoVisibility =>
    TopPlayer != null && TopPlayer.CardCount == 1 &&
    !TopPlayer.UnoPressed
        ? Visibility.Visible
        : Visibility.Collapsed;

        public Visibility LeftPlayerUnoVisibility =>
            LeftPlayer != null && LeftPlayer.CardCount == 1 &&
    !LeftPlayer.UnoPressed
                ? Visibility.Visible
                : Visibility.Collapsed;

        public Visibility MePlayerUnoVisibility =>
             MePlayer != null && MePlayer.CardCount == 1
                 ? Visibility.Visible
                 : Visibility.Collapsed;

        public Brush TopPlayerBorderColor =>
    TopPlayer != null && 2 == state.currentPlayer
        ? Brushes.Green
        : Brushes.Gray;


        public Brush LeftPlayerBorderColor =>
            LeftPlayer != null && 1 == state.currentPlayer
                ? Brushes.Green
                : Brushes.Gray;


        public Brush RightPlayerBorderColor =>
            RightPlayer != null && 3 == state.currentPlayer
                ? Brushes.Green
                : Brushes.Gray;


        public Brush MePlayerBorderColor =>
            MePlayer != null && 0 == state.currentPlayer
                ? Brushes.Green
                : Brushes.Gray;

        public ImageSource MyAvatar =>
    AvatarHelper.GetAvatar(App.Session.Client.player.AvatarId);

        public GameViewModel()
        {
            state = new GameState();

            DrawCardCommand = new RelayCommand(() => _ =  TakeCardAsync());
            PlayCardCommand = new RelayCommand<Card>(card => _ = PlayCardCommandExecute(card));
            UnoCommand = new RelayCommand<PlayerInfo>(player =>
            {
                if (player == null)
                    return;

                _ = PlayUnoAsync(player.Id);
            });

            ChooseColorCommand = new RelayCommand<ColorCard>(async color =>
            {
                if (_selectedCard == null)
                    return;
                
                await PlayCardAsync(_selectedCard, color);
                _selectedCard = null;
                IsColorSelectionVisible = false;
            });
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
                case StartGameMessageToClient gamestage:
                    StartGameToClient?.Invoke(gamestage);
                    break;
                    
                case GameStateMessage game:
                    InitializeGame(game);
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
                case RequestType.EditProfile:
                    _editProfileTcs?.SetResult(confirm);
                    _editProfileTcs = null;
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
                    CanStartGame?.Invoke(confirm);
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
                case RequestType.EditProfile:
                    _editProfileTcs?.SetException(new Exception(error.TextError));
                    _editProfileTcs = null;
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
                    ProcessGameError(error);
                    break;
                case RequestType.TakeCard:
                    ProcessGameError(error);
                    _takeCardTcs?.SetException(new Exception(error.TextError));
                    _takeCardTcs = null;
                    break;
                case RequestType.PlayCard:
                    ProcessGameError(error);
                    _playCardTcs?.SetException(new Exception(error.TextError));
                    _playCardTcs = null;
                    break;
                case RequestType.PlayUno:
                    ProcessGameError(error);
                    _playUnoTcs?.SetException(new Exception(error.TextError));
                    _playUnoTcs = null;
                    break;
            }
        }
        private void ProcessGameError(ErrorMessage error)
        {
            ShowError(error.TextError);
        }
        public void InitializeGame(GameStateMessage game)
        {
            state.Update(game);
            if (!_startMessageShown)
            {
                ShowFirstPlayerMessage();
                _startMessageShown = true;
            }
            CheckTopCardNotification();

            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(TopCard));
            OnPropertyChanged(nameof(MyCardsCount));
            OnPropertyChanged(nameof(CanSayUno));

            OnPropertyChanged(nameof(TopPlayer));
            OnPropertyChanged(nameof(LeftPlayer));
            OnPropertyChanged(nameof(RightPlayer));
            OnPropertyChanged(nameof(MePlayer));

            OnPropertyChanged(nameof(TopPlayerUnoVisibility));
            OnPropertyChanged(nameof(LeftPlayerUnoVisibility));
            OnPropertyChanged(nameof(RightPlayerUnoVisibility));
            OnPropertyChanged(nameof(MePlayerUnoVisibility));

            OnPropertyChanged(nameof(TopPlayerBorderColor));
            OnPropertyChanged(nameof(LeftPlayerBorderColor));
            OnPropertyChanged(nameof(RightPlayerBorderColor));
            OnPropertyChanged(nameof(MePlayerBorderColor));
            OnPropertyChanged(nameof(SelectedColorText));

        }
        private void ShowFirstPlayerMessage()
        {
            if (state.Players.Count == 0)
                return;

            var firstPlayer = state.Players[state.currentPlayer];

            _ = ShowNotification(
                $"{firstPlayer.Name} goes first"
            );
        }
        private Card _previousTopCard;
        private void CheckTopCardNotification()
        {
            var currentCard = state.lastCard;


            if (currentCard == null)
                return;


            if (_previousTopCard == null)
            {
                _previousTopCard = currentCard;
                return;
            }


            if (_previousTopCard.Type != currentCard.Type ||
               _previousTopCard.Color != currentCard.Color)
            {
                string message = GetCardNotification(currentCard);


                if (message != null)
                {
                    _ = ShowNotification(message);
                }
            }


            _previousTopCard = currentCard;
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
        public async Task<ConfirmMessage> SignUpAsync(string name, string password, int avatarId)
        {
            _signUpTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new SignUpMessage(name, password, avatarId));

            return await _signUpTcs.Task;
        }
        public async Task<ConfirmMessage> EditProfileAsync(string name, string password, int avatarId)
        {
            _editProfileTcs = new TaskCompletionSource<ConfirmMessage>();
            
            await App.Session.Client.SendAsync(new EditProfileMessage(name, password, avatarId));

            return await _editProfileTcs.Task;
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
            if (card.Color == ColorCard.Black && MePlayerBorderColor == Brushes.Green)
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
        public async Task<ConfirmMessage> PlayUnoAsync(Guid Id)
        {
            Logger.Log($"UNO pressed. Id = {Id}");
            _playUnoTcs = new TaskCompletionSource<ConfirmMessage>();

            await App.Session.Client.SendAsync(new PushUNOMessage(Id));

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
