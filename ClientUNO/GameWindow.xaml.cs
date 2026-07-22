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
        // карты не убираются из руки при ходе игрока
        // Action в начале хода игрока не работает
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
