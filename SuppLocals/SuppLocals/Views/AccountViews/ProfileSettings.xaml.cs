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

        private string strName;
        private string imageName;

        public ProfileSettings(User user)
        {
            InitializeComponent();

            ActiveUser = user;
            showImage();
        }

        private void ProfileImageClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.png;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    strName = fldlg.SafeFileName;
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    profileImage.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
                }
                fldlg = null;

                insertImageData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
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

            showImage();
            return;
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void showImage()
        {
            using UsersDbTable db = new UsersDbTable();


            //Store binary data read from the database in a byte array
            byte[] blob = db.Users.Single(x => x.ID == ActiveUser.ID).Image;

            if (blob == null)
            {
                profileImage.ImageSource = new BitmapImage(new Uri(@"C:\Users\Paulius\Desktop\Studijos\.NET\1st Semester\MasterBranch\SuppLocals\SuppLocals\Assets\profile.png"));
            }
            else
            {

                MemoryStream stream = new MemoryStream();
                stream.Write(blob, 0, blob.Length);
                stream.Position = 0;

                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                profileImage.ImageSource = bi;
            }

        }

        private void insertImageData()
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
