using Shared_Library;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

namespace ClientUNO
{
    public partial class GameOverWindow : Window
    {

        // сделать в gameover двойной клик по игроку, чтобы открывался профиль
        // не видно очков игроков
        // мб добавить красивую надпись победитель над первым игроком
        public ObservableCollection<PlayerInfo> Players { get; set; }
        public GameViewModel Model { get; }
        public GameClient Client { get; }
        public GameOverWindow()
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;

            Players = new ObservableCollection<PlayerInfo>();
            DataContext = this;
            var sorted = Model.state.Players.OrderBy(p => p.Points).ToList();

            Players.Clear();

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].IsWinner = (i == 0);
                Players.Add(sorted[i]);
            }
            foreach(var p in Players)
            {
                Logger.Log($"{p.Name} {p.Id} {p.Wins} {p.CountGames}");
            }
        }

        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            Lobby lobby = new Lobby(App.Session.Client.roomCode);
            lobby.Show();

            Close();
        }
    }
}