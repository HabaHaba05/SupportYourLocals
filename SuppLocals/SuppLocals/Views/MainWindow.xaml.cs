using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using SuppLocals.ViewModels;


namespace SuppLocals.Views
{
    public partial class MainWindow : Window
    {
        //The user
        public User ActiveUser;

        public MainWindow(User user)
        {
            //By default
            InitializeComponent();

            ActiveUser = user;
            ProfileUser.Text = ActiveUser.Username;

            DataContext = new HomeVM(this, ActiveUser);

            MyImage.ImageSource = ActiveUser.GetProfileImage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActiveUser.Username == "Anonimas")
            {
                AnonymTabs.Visibility = Visibility.Visible;
                VendorTabs.Visibility = Visibility.Hidden;
            }
            else
            {
                AnonymTabs.Visibility = Visibility.Hidden;
                VendorTabs.Visibility = Visibility.Visible;
            }
        }

        #region Menu methods

        private void TabClicked(object sender, RoutedEventArgs e)
        {
            var index = int.Parse(((Button) e.Source).Uid);

            ProfilePan.Visibility = Visibility.Collapsed;
            profileButton.Background = new SolidColorBrush(Color.FromRgb(204, 186, 139));

            var ta = new ThicknessAnimation
            {
                From = TabCursor.Margin,
                To = new Thickness(120 * index, 0, 0, 10),
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };
            TabCursor.BeginAnimation(MarginProperty, ta);

            DataContext = index switch
            {
                0 => new HomeVM(this, ActiveUser),
                1 when ActiveUser.Username == "Anonimas" => new VendorsVM(),
                1 => new MyServicesVM(ActiveUser),
                2 when ActiveUser.Username == "Anonimas" => new FaqVM(),
                2 => new AddServiceVM(ActiveUser),
                _ => new AboutVM()
            };
        }

        private void ProfileClicked(object sender, RoutedEventArgs e)
        {
            if (ProfilePan.Visibility == Visibility.Collapsed)
            {
                ProfilePan.Visibility = Visibility.Visible;

                if (ActiveUser.Username == "Anonimas")
                {
                    profileButton.Background = new SolidColorBrush(Color.FromRgb(250, 250, 249));
                    LogOutPanel.Visibility = Visibility.Hidden;
                    SignInPanel.Visibility = Visibility.Visible;
                }

                else
                {
                    profileButton1.Background = new SolidColorBrush(Color.FromRgb(250, 250, 249));
                    SignInPanel.Visibility = Visibility.Hidden;
                    LogOutPanel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ProfilePan.Visibility = Visibility.Collapsed;
                profileButton.Background = new SolidColorBrush(Color.FromRgb(204, 186, 139));
                profileButton1.Background = new SolidColorBrush(Color.FromRgb(204, 186, 139));
            }
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) {UseShellExecute = true});
            e.Handled = true;
        }

        #endregion

        #region ProfileMethods

        private void ProfileSettingsClicked(object sender, RoutedEventArgs e)
        {
            DataContext = new ChangeProfileVM(ActiveUser);
            ProfilePan.Visibility = Visibility.Collapsed;
            profileButton1.Background = new SolidColorBrush(Color.FromRgb(204, 186, 139));         
            MyImage.ImageSource = ActiveUser.GetProfileImage();
        }

        private void LogOutClicked(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Username = "Anonimas",
                VendorsCount = 0,
                HashedPsw = ""
            };

            var map = new MainWindow(user);
            map.Show();
            Close();
        }

        private void SignInClicked(object sender, RoutedEventArgs e)
        {
            var lWindow = new Login();
            lWindow.Show();
            Close();
        }

        #endregion
    }
}