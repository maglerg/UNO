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
        // при нажатии на кнопку уно ничего не просиходит
        // нет адекватного конца игры
        // прокачать интерфейс. 
            // проверить везде вывод ошибок
            // подправить и сделать кнопки и окна более аккуратными
            // сделать ошибки более понятными для пользователя
            // в лобби сделать сбоку более красивое меню с иконками и добавить обозначение игрока хостом
        // сделать настройки для игры 
        public GameWindow(GameStateMessage state)
        {
            InitializeComponent();

            DataContext = App.Session.Model;

            App.Session.Model.InitializeGame(state);
        }
    }
}
