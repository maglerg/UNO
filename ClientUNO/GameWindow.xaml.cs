using Shared_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class GameWindow : Window
    {
        // если один из игроков вышел то игра заканчивается и игрок возвращается в лобби комнаты + появляется сообщение что игрок отключился
        //при броске дикой карты 4 не понятно какой цвет выбал игрок тк видна только надпись что игрок берет 4 карты
        // доделать интерфейс с окном игры (иконки + имена и норм количество карт)
        // в лобби баг если хост игры выходит нет обозначения нового хоста и нет кнопки старт
        // сделать настройки для игры 
        public GameWindow(GameStateMessage state)
        {
            InitializeComponent();

            DataContext = App.Session.Model;

            App.Session.Model.InitializeGame(state);

            App.Session.Model.GameEnded += OnGameEnded;
        }
        private void OnGameEnded()
        {
            Dispatcher.Invoke(() =>
            {
                GameOverWindow window = new GameOverWindow();
                window.Show();
                Close();
            });
        }
    }
}
