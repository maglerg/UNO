using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Shared_Library;

namespace ClientUNO
{
    public partial class PlayerMenu : Window
    {
        public GameViewModel Model { get; }
        public GameClient Client { get; }
        public PlayerMenu()
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;

            username.Text = Client.player.Name;
        }
        private async void CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirm = await App.Session.Model.CreateRoomAsync();

                Lobby window = new Lobby(confirm.roomCode);
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                CreateRoomError.Text = ex.Message;
            }
        }

        private async void ConnectRoom_Click(object sender, RoutedEventArgs e)
        {
            string code = RoomCode.Text;

            try
            {
                var confirm = await App.Session.Model.ConnectRoomAsync(code);

                Lobby window = new Lobby(confirm.roomCode);
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                ConnectRoomError.Text = ex.Message;
            }
        }

        private async void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}
