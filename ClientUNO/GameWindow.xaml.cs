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
        //при броске дикой карты 4 не опнятно какой цвет выбал игрок тк видна только надпись что игрок берет 4 карты
        // при нажатии на кнопку уно ничего не просиходит
        // нет адекватного конца игры
        // прокачать интерфейс. 
            // проверить везде вывод ошибок
            // подправить и сделать кнопки и окна более аккуратными
            // сделать ошибки более понятными для пользователя
            // в лобби добавить обозначение игрока хостом
            // переименовать выходы везде в exit
        // сделать настройки для игры 
        public GameWindow(GameStateMessage state)
        {
            InitializeComponent();

            DataContext = App.Session.Model;

            App.Session.Model.InitializeGame(state);
        }
    }
}
