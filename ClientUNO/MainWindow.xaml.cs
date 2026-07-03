using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientUNO
{
    public partial class MainWindow : Window
    {
        public GameViewModel Model { get;}
        public GameClient Client { get; }
        public MainWindow()
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;
        }
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp window = new SignUp();
            window.Owner = this;
            window.Show();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login window = new Login();
            window.Owner = this;
            window.Show();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}