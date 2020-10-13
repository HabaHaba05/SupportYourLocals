using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Drawing.Imaging;
using System.Windows.Controls;
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

        
        private string imageName;

        public ProfileSettings(User user)
        {
            InitializeComponent();

            ActiveUser = user;
            profileImage.ImageSource = ActiveUser.GetProfileImage();
        }


        private void ProfileImageClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog
                {
                    InitialDirectory = Environment.SpecialFolder.MyPictures.ToString(),
                    Filter = "Image File (*.jpg;*.png;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif"
                };
                fldlg.ShowDialog();
                {
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    profileImage.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
                    
                }
                fldlg = null;

                InsertImageData();
                profileImage.ImageSource = ActiveUser.GetProfileImage();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        // Only applies to password changes
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
            return;
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void InsertImageData()
        {
            try
            {
                if (imageName != "")
                {
                    //Initialize a file stream to read the image file
                    FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

                    //Initialize a byte array with size of stream
                    byte[] imgByteArr = new byte[fs.Length];

                    //Read data from the file stream and put into the byte array
                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

                    //Close a file stream
                    fs.Close();

                    using (UsersDbTable db = new UsersDbTable())
                    {
                        var user = db.Users.SingleOrDefault(x => x.ID == ActiveUser.ID);

                        user.Image = imgByteArr;
                        ActiveUser.Image = imgByteArr;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
