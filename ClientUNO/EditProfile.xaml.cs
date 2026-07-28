using Shared_Library;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ClientUNO
{
    public partial class EditProfile : Window
    {
        public GameViewModel Model { get; }
        public GameClient Client { get; }
        private int _currentAvatarId;
        private int? _newAvatarId;
        public EditProfile()
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;

            UsernameBox.Text = Client.player.Name;
            StatusBox.Text = Client.player.Status;

            _currentAvatarId = App.Session.Client.player.AvatarId;
            AvatarImage.Source = AvatarHelper.GetAvatar(_currentAvatarId);
            Logger.Log($"при создании EditProfile{App.Session.Client.player.Id.ToString()}");
        }
        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            ChooseAvatar window = new ChooseAvatar();

            if (window.ShowDialog() == true)
            {
                _newAvatarId = window.SelectedAvatar.Value;

                AvatarImage.Source = AvatarHelper.GetAvatar(_newAvatarId.Value);
            }
        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            bool usernameIsChange = false;
            var newUsername = UsernameBox.Text;
            var newStatus = StatusBox.Text;
            int newAvatar = Client.player.AvatarId;
             bool hasError = false;
            if (newUsername.Length < 3)
            {
                ErrorText.Text = "Username must be at least 3 characters";
                ErrorText.Visibility = Visibility.Visible;
                hasError = true;
            }
            else
            {
                ErrorText.Visibility = Visibility.Collapsed;
            }

            try
            {
                if (usernameIsChange)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Are you sure you want to change your username?\n\n" +
                        "You are changing the name displayed in the game, but your account login username will not be changed.",
                        "Confirm Username Change",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                if (_newAvatarId != null)
                {
                    newAvatar = _newAvatarId.Value;
                }
                Logger.Log($"пееред отправкой в EditProfile{App.Session.Client.player.Id.ToString()}");
                var confirm = await App.Session.Model.EditProfileAsync(newUsername, newStatus, newAvatar);
                
                Client.player.Name = newUsername;
                Client.player.AvatarId = newAvatar;
                Client.player.Status = newStatus;
                Logger.Log("User's data has been changed");
                PlayerMenu lobby = new PlayerMenu();
                lobby.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
                ErrorText.Visibility = Visibility.Visible;
            }

        }
        private void Exit_Click(object sender, EventArgs e)
        {
            PlayerMenu lobby = new PlayerMenu();
            lobby.Show();

            this.Close();
        }
        
    }
}
