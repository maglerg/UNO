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
        public GameViewModel Model { get; }
        public GameClient Client { get; }
        private Card selectedCard;

        public ICommand DrawCardCommand { get; }
        public ICommand PlayCardCommand { get; }
        public ICommand UnoCommand { get; }

        public string ErrorMessage { get; set; }
        public Visibility ErrorVisibility { get; set; } = Visibility.Collapsed;
        public event PropertyChangedEventHandler PropertyChanged;

        public GameWindow()
        {
            InitializeComponent();
            Client = App.Session.Client;
            Model = App.Session.Model;

            Model.GameStateChanged += UpdateUI;

            DrawCardCommand = new RelayCommand(() => _ = DrawCard());
            PlayCardCommand = new RelayCommand(obj => PlayCard(obj));
            UnoCommand = new RelayCommand(() => _ = PlayUno());
        }

        private async Task DrawCard()
        {
            try
            {
                await App.Session.Model.TakeCardAsync();
                
            }
            catch (Exception ex)
            {
                Model.ShowError(ex.Message);
            }
        }
        private async Task PlayCard(object obj)
        {
            try
            {
                selectedCard = (Card)obj;

                if (selectedCard.Color == ColorCard.Black) 
                {
                    ColorOverlay.Visibility = Visibility.Visible;
                    return;
                }

                SendCard(selectedCard, null);

            }
            catch (Exception ex)
            {
                Model.ShowError(ex.Message);
            }
        }
        private async Task PlayUno()
        {
            try
            {
                await App.Session.Model.PlayUnoAsync();

            }
            catch (Exception ex)
            {
                Model.ShowError(ex.Message);
            }
        }
        private void ChooseRed(object sender, RoutedEventArgs e)
        {
            SendCard(selectedCard, ColorCard.Red);
        }

        private void ChooseBlue(object sender, RoutedEventArgs e)
        {
            SendCard(selectedCard, ColorCard.Blue);
        }

        private void ChooseGreen(object sender, RoutedEventArgs e)
        {
            SendCard(selectedCard, ColorCard.Green);
        }

        private void ChooseYellow(object sender, RoutedEventArgs e)
        {
            SendCard(selectedCard, ColorCard.Yellow);
        }
        private void SendCard(Card card, ColorCard? color)
        {
            ColorOverlay.Visibility = Visibility.Collapsed;

            _ = Model.PlayCardAsync(card, color);
        }
        public void ShowError(string message)
        {
            ErrorMessage = message;
            ErrorVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(ErrorVisibility));

            Task.Delay(2000).ContinueWith(_ =>
            {
                ErrorVisibility = Visibility.Collapsed;
                OnPropertyChanged(nameof(ErrorVisibility));
            });
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void UpdateUI()
        {
            var state = App.Session.Model.state;
            // пример: твои карты
            CardsControl.ItemsSource = state.MyCards;

            // текстовые поля (если есть)
            MyCardsCountText.Text = state.MyCards.Count.ToString();

            // последний ход
            LastCardImage.Source = new BitmapImage(
                new Uri(state.lastCard.ImagePath, UriKind.Relative)
            );

            // твой ход
            UnoButton.Visibility = state.MyCards.Count == 1
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}
