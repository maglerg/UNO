using Shared_Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientUNO
{
    public partial class Lobby : Window
    {
        public ObservableCollection<PlayerInfo> Players { get; set; }
        public string roomCode { get; set; }
        public GameViewModel Model { get; }
        public GameClient Client { get; }
        public Lobby(string roomCode)
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;

            this.roomCode = roomCode;
            Players = new ObservableCollection<PlayerInfo>();
            Players.Add(Client.player);
            roomcode.Text = roomCode;
            DataContext = this;
            App.Session.Model.CanStartGame += OnStartGame;
            App.Session.Model.StartGameError += OnStartGameError;
            App.Session.Model.SomeoneJoin += OnSomeoneJoinGame;
            App.Session.Model.SomeoneLeft += OnSomeoneLeftGame;
            App.Session.Model.ChangeHost += OnChangeHost;
            Model.StartGameToClient += OnStartGameToClient;

            if (Client.player.IsHost)
            {
                StartGame.Visibility = Visibility.Visible;
            } 
            else
                StartGame.Visibility = Visibility.Collapsed;

            _ = RequestPlayersAsync(); // получаем остальных игроков
        }
        private async Task RequestPlayersAsync()
        {
            await Client.SendAsync(new RequestPlayersMessage());
        }
        private void OnChangeHost(ChangeHostMessage mess)
        {
            Logger.Log($"Host is change.");

            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    var p = Players[i];

                    bool isNewHost = p.Id == mess.player.Id;

                    Players[i] = new PlayerInfo(
                        p.Id,
                        p.Name,
                        p.CardCount,
                        isNewHost,
                        p.Status,
                        p.AvatarId,
                        p.Wins,
                        p.CountGames,
                        p.Points,
                        p.UnoPressed
                    );
                }

                // Показываем кнопку только новому хосту
                StartGame.Visibility =
                    Client.player.Id == mess.player.Id
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            });
        }
        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            CanStartGameMessage msg = new CanStartGameMessage();

            await Client.SendAsync(msg);
        }
        private async void OnStartGame(ConfirmMessage message)
        {
            await Client.SendAsync(new StartGameMessageToServer());
            Logger.Log($"Game start");
        }
        private void OnStartGameToClient(StartGameMessageToClient msg)
        {
            Dispatcher.Invoke(() =>
            {
                GameWindow window = new GameWindow(msg.GameState);
                window.Show();
                Close();
            });
        }
        private async void OnStartGameError(ErrorMessage error)
        {
            Dispatcher.Invoke(() =>
            {
                StartGameError.Text = error.TextError;
                StartGameError.Visibility = Visibility.Visible;
            });

            await Task.Delay(3000);

            Dispatcher.Invoke(() =>
            {
                StartGameError.Visibility = Visibility.Collapsed;
            });
            Logger.Log($"Cant start game");
            Logger.Error(error.TextError);
        }
        private void OnSomeoneJoinGame(SomeoneJoinMessage msg)
        {
            Logger.Log($" {msg.Player.Name} join game");

            Application.Current.Dispatcher.Invoke(() =>
            {
                Players.Add(msg.Player);
                Logger.Log($"Players count = {Players.Count}");
            });
        }
        private void OnSomeoneLeftGame(SomeoneLeftMessage msg)
        {
            Logger.Log($"{msg.Player.Name} left game");

            Application.Current.Dispatcher.Invoke(() =>
            {
                var player = Players.FirstOrDefault(p => p.Id == msg.Player.Id);

                if (player != null)
                {
                    Players.Remove(player);
                }
                Logger.Log($"Players count = {Players.Count}");
            });
        }
        protected override void OnClosed(EventArgs e)
        {
            App.Session.Model.CanStartGame -= OnStartGame;
            App.Session.Model.StartGameError -= OnStartGameError;
            App.Session.Model.SomeoneJoin -= OnSomeoneJoinGame;
            App.Session.Model.SomeoneLeft -= OnSomeoneLeftGame;

            base.OnClosed(e);
        }

        public async void LeaveGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirm = await App.Session.Model.LeftRoomAsync();
                Client.roomCode = null;
                PlayerMenu window = new PlayerMenu();
                window.Show();
                Close();
            }
            catch (Exception ex)
            {

                Dispatcher.Invoke(() =>
                {
                    LeaveGameError.Text = ex.Message;
                    LeaveGameError.Visibility = Visibility.Visible;
                });

                await Task.Delay(3000);

                Dispatcher.Invoke(() =>
                {
                    LeaveGameError.Visibility = Visibility.Collapsed;
                });
                
            }
        }
        private async void CopyRoomCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(roomcode.Text);
            roomcopy.Visibility = Visibility.Visible;

            await Task.Delay(1500);

            roomcopy.Visibility = Visibility.Collapsed;
        }
        private void PlayersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListBox)sender).SelectedItem is PlayerInfo player)
            {
                PlayerProfile window = new PlayerProfile(player);
                window.ShowDialog();
            }
        }
    }
}
