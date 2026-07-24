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
                    ? "Images/Wild.png"
                    : "Images/WildFour.png";
            }

            string color = card.Color.ToString();

            return card.Type switch
            {
                TypeCard.Number => $"Images/{color}_{card.Value}.png",
                TypeCard.Skip => $"Images/{color}_Block.png",
                TypeCard.Reverse => $"Images/{color}_Reverse.png",
                TypeCard.DrawTwo => $"Images/{color}_PlusTwo.png",
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
