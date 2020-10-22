using Microsoft.Win32;
using SuppLocals.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuppLocals.Views
{
    public partial class ChangeProfile : UserControl
    {
        private string _imageName;
        public User ActiveUser;

        public ChangeProfile()
        {
            InitializeComponent();

            ActiveUser = ChangeProfileVM.ActiveUser;
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
                    _imageName = fldlg.FileName;
                    var isc = new ImageSourceConverter();

                    profileImage.SetValue(System.Windows.Controls.Image.SourceProperty, isc.ConvertFromString(_imageName));
                }
                fldlg = null;

                InsertImageData();
                profileImage.ImageSource = ActiveUser.GetProfileImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertImageData()
        {
            try
            {
                if (_imageName == "")
                {
                    return;
                }

                //Initialize a file stream to read the image file
                var fs = new FileStream(_imageName, FileMode.Open, FileAccess.Read);

                //Initialize a byte array with size of stream
                var imgByteArr = new byte[fs.Length];

                //Read data from the file stream and put into the byte array
                fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

                //Close a file stream
                fs.Close();

                using (var db = new UsersDbTable())
                {
                    var user = db.Users.SingleOrDefault(x => x.ID == ActiveUser.ID);

                    user.Image = imgByteArr;
                    ActiveUser.Image = imgByteArr;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}