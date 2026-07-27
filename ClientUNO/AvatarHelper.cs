using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientUNO
{
    public static class AvatarHelper
    {
        public static ImageSource GetAvatar(int avatarId)
        {
            return new BitmapImage(
                new Uri($"Images/Avatars/{avatarId}.jpg", UriKind.Relative));
        }
    }
}
