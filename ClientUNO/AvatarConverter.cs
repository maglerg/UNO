using System;
using System.Globalization;
using System.Windows.Data;

namespace ClientUNO
{
    public class AvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is int avatarId)
                return AvatarHelper.GetAvatar(avatarId);

            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}