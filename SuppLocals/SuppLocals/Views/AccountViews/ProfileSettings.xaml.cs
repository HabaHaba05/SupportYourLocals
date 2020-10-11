using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.Views.AccountViews
{
    /// <summary>
    /// Interaction logic for ProfileSettings.xaml
    /// </summary>
    public partial class ProfileSettings : Window
    {
        public User ActiveUser;

        public ProfileSettings(User user)
        {
            InitializeComponent();

            ActiveUser = user;
        }

        private void ProfileImageClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                InitialDirectory = "c:\\desktop",
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp",
                RestoreDirectory = true
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string selectedFileName = dlg.FileName;

                ImageBrush myBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(selectedFileName))
                };
                PrfImage.Fill = myBrush;
            }
        }

        private void SaveChangesClicked(object sender, RoutedEventArgs e)
        {
            var oldPassword = OldPass.Password.ToString();
            var newPassword = NewPass.Password.ToString();
            var newPasswordConfirm = ConfirmNewPass.Password.ToString();

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(newPasswordConfirm))
            {
                EmptyFields.Visibility = Visibility.Visible;
                return;
            }
            EmptyFields.Visibility = Visibility.Hidden;


            if(oldPassword == newPassword)
            {
                OldMatchesNew.Visibility = Visibility.Visible;
                return;
            }
            OldMatchesNew.Visibility = Visibility.Hidden;


            if (newPassword != newPasswordConfirm)
            {
                PasswordsDontMatch.Visibility = Visibility.Visible;
                return;
            }
            PasswordsDontMatch.Visibility = Visibility.Hidden;

            if(!BC.Verify(oldPassword, ActiveUser.HashedPsw))
            {
                IncorrectOldPass.Visibility = Visibility.Visible;
                return;
            }
            IncorrectOldPass.Visibility = Visibility.Hidden;

            //changing the current password to new password
            using (UsersDbTable dbUser = new UsersDbTable())
            {
                var user = dbUser.Users.SingleOrDefault(x => x.ID == ActiveUser.ID);
                user.HashedPsw = BC.HashPassword(newPassword);
                dbUser.SaveChanges();
            }

            OldPass.Clear();
            NewPass.Clear();
            ConfirmNewPass.Clear();

            MessageBox.Show("Password changed successfully.");
            return;
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
