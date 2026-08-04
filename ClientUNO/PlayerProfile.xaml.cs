using Shared_Library;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
    public partial class PlayerProfile : Window
    {
        public PlayerProfile(PlayerInfo player)
        {
            InitializeComponent();

            username.Text = player.Name;
            status.Text = player.Status;
            avatar.Source = AvatarHelper.GetAvatar(player.AvatarId);

            wins.Text = player.Wins.ToString();
            // rating.Text 
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
