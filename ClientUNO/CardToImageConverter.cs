using Shared_Library;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ClientUNO.Converters
{
    public class CardToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Card card)
                return null;

            if (card.Color == ColorCard.Black)
            {
                return card.Type == TypeCard.Wild
                    ? "Images/Cards/Wild.png"
                    : "Images/Cards/WildFour.png";
            }

            string color = card.Color.ToString();

            return card.Type switch
            {
                TypeCard.Number => $"Images/Cards/{color}_{card.Value}.png",
                TypeCard.Skip => $"Images/Cards/{color}_Block.png",
                TypeCard.Reverse => $"Images/Cards/{color}_Reverse.png",
                TypeCard.DrawTwo => $"Images/Cards/{color}_PlusTwo.png",
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
