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
        GameClient Client {  get; set; }
        public PlayerMenu()
        {
            InitializeComponent();
            Client.player = new Player("Sigma_Woman");
            username.Text = Client.player.Name;
        }

        public async Task CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomMessage msg= new CreateRoomMessage();
            await SendAsync(msg);

            // схвать ответ с кодом комнаты и создать окно комнаты с переданным кодом
        }

        public async Task ConnectRoom_Click(object sender, RoutedEventArgs e)
        {
            // открыть окошко для вписывания кода ( или сделать его здесь же )
            // получить код комнаты
            // получить подтверждение что комната найдена
           
            //  создать окно комнаты с переданным кодом
        }
    }
}
