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
        // при нажатии на кнопку уно ничего не просиходит
        //некоторые карты не отображаются нормально. нужно перепроверить названия и свойства
        // нет адекватного конца игры
        // баги с дикими картами неправильное отображение и выбор цвета зависает
        // проблемы в разметке окна. все криво косо
        public GameWindow(GameStateMessage state)
        {
            InitializeComponent();

            DataContext = App.Session.Model;

            App.Session.Model.InitializeGame(state);
        }
    }
}
