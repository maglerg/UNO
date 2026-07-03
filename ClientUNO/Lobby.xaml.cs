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
            var player = new PlayerInfo(Client.player.Name, -1);
            Players.Add(player);
            roomcode.Text = roomCode;
            DataContext = this;

            App.Session.Model.StartGame += OnStartGame;
            App.Session.Model.StartGameError += OnStartGameError;
            App.Session.Model.SomeoneJoin += OnSomeoneJoinGame;
            App.Session.Model.SomeoneLeft += OnSomeoneLeftGame;
        }
       

        public async Task StartGame_Click()
        {
            StartGameMessage msg = new StartGameMessage();
            await Client.SendAsync(msg);
        }
        private void OnStartGame(ConfirmMessage confirm)
        {
            Logger.Log("Game start");
            GameWindow window = new GameWindow();
            window.Show();
            Close();
        }
        private void OnStartGameError(ErrorMessage error)
        {
            StartGameError.Text = error.TextError;
            Logger.Log("Cant start game");
        }
        private void OnSomeoneJoinGame(SomeoneJoinMessage msg)
        {
            Players.Add(msg.Player);
            Logger.Log($"{msg.Player} join game");
        }
        private void OnSomeoneLeftGame(SomeoneLeftMessage msg)
        {
            Players.Remove(msg.Player);
            Logger.Log($"{msg.Player} left game");
        }
        protected override void OnClosed(EventArgs e)
        {
            App.Session.Model.StartGame -= OnStartGame;
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

                PlayerMenu window = new PlayerMenu();
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                LeaveGameError.Text = ex.Message;
            }
        }
        private void CopyRoomCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(roomcode.Text);
        }

    }
}
