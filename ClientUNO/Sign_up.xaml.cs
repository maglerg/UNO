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

namespace ClientUNO
{
    /// <summary>
    /// Логика взаимодействия для Sign_up.xaml
    /// </summary>
    public partial class Sign_up : Window
    {
        public Sign_up()
        {
            InitializeComponent();
        }
        public void SignUp_Click (object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password1 = Password1.Password;
            string password2 = Password2.Password;

            bool hasError = false;

            if (username.Length < 3)
            {
                UsernameError.Text = "Username must be at least 3 characters";
                UsernameError.Visibility = Visibility.Visible;
                hasError = true;
            }
            else
            {
                UsernameError.Visibility = Visibility.Collapsed;
            }

            if (password1.Length < 6)
            {
                PasswordError1.Text = "Password must be at least 6 characters";
                PasswordError1.Visibility = Visibility.Visible;
                hasError = true;
            }
            else
            {
                PasswordError1.Visibility = Visibility.Collapsed;
            }

            if (password1 != password2)
            {
                PasswordError.Text = "Passwords do not match";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }
            else
            {
                PasswordError.Visibility = Visibility.Collapsed;
            }

            if (!hasError)
            {
                Lobby lobby = new Lobby();
                lobby.Show();

                this.Owner.Close();
                this.Close();
            }
        }
        public void Undo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
