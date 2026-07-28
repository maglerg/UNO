using System;
using System.Globalization;
using System.Windows.Data;

namespace ClientUNO
{
    public class PlayerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid playerId)
            {
                return playerId == App.Session.Client.player.Id;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}