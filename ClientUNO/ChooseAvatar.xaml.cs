using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ChooseAvatar : Window
    {
        private Border? selectedAvatar;
        public int? SelectedAvatar { get; private set; }
        public ChooseAvatar()
        {
            InitializeComponent();
            LoadAvatars();
        }
        private void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            Border clickedAvatar = (Border)sender;


            // убрать рамку с предыдущего выбора
            if (selectedAvatar != null)
            {
                selectedAvatar.BorderBrush = null;
                selectedAvatar.BorderThickness = new Thickness(0);
            }


            // добавить рамку новому выбору
            clickedAvatar.BorderBrush = Brushes.SaddleBrown;
            clickedAvatar.BorderThickness = new Thickness(3);


            selectedAvatar = clickedAvatar;


            // сохранить номер аватара
            SelectedAvatar = (int)clickedAvatar.Tag;
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAvatar == null)
            {
                ErrorText.Text = "Please select an avatar";
                return;
            }

            DialogResult = true;
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void LoadAvatars()
        {
            string folder = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "Avatars");

            if (!Directory.Exists(folder))
                return;

            var files = Directory.GetFiles(folder, "*.jpg")
                                 .OrderBy(path =>
                                 {
                                     string name = System.IO.Path.GetFileNameWithoutExtension(path);
                                     return int.TryParse(name, out int number)
                                         ? number
                                         : int.MaxValue;
                                 });

            foreach (string file in files)
            {
                Border border = new Border
                {
                    Width = 120,
                    Height = 120,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(8),
                    Cursor = Cursors.Hand,
                    Tag = int.Parse(System.IO.Path.GetFileNameWithoutExtension(file))
                };

                Image image = new Image
                {
                    Stretch = Stretch.UniformToFill,
                    Source = new BitmapImage(new Uri(file))
                };

                border.Child = image;
                border.MouseLeftButtonDown += Avatar_Click;
                AvatarPanel.Children.Add(border);
            }
        }

    }
}
