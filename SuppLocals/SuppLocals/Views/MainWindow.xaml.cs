using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Location = Microsoft.Maps.MapControl.WPF.Location;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Linq;
using SuppLocals.ViewModels;
using SuppLocals.Views.AccountViews;

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
            int index = int.Parse(((Button)e.Source).Uid);

            ProfilePan.Visibility = Visibility.Collapsed;
            profileButton.Background = new SolidColorBrush(Color.FromRgb(204, 186, 139));

            ThicknessAnimation ta = new ThicknessAnimation
            {
                From = TabCursor.Margin,
                To = new Thickness((120 * index), 0, 0, 10),
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };
            TabCursor.BeginAnimation(Button.MarginProperty, ta);

            if (index.Equals(0))
            {
                DataContext = new HomeVM(this,ActiveUser);
            }

            else if (index.Equals(1)) 
            { 
                if (ActiveUser.Username == "Anonimas") { DataContext = new VendorsVM(); }
                else { DataContext = new MyServicesVM(ActiveUser); }
            }
            
            else if (index.Equals(2)) 
            {
                if (ActiveUser.Username == "Anonimas") { DataContext = new FaqVM(); } 
                else { DataContext = new AddServiceVM(ActiveUser); }
            }
            else { DataContext = new AboutVM(); }
        }
        
        private void ProfileClicked(object sender, RoutedEventArgs e)
        {
            if (ProfilePan.Visibility == Visibility.Collapsed)
            {
                ProfilePan.Visibility = Visibility.Visible;

                if(ActiveUser.Username == "Anonimas")
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
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        #endregion

        #region ProfileMethods
        private void ProfileSettingsClicked(object sender, RoutedEventArgs e)
        {
            ProfileSettings profile = new ProfileSettings(ActiveUser);
            ProfilePan.Visibility = Visibility.Collapsed;
            profile.ShowDialog();
            MyImage.ImageSource = ActiveUser.GetProfileImage();
        }

        private void LogOutClicked(object sender, RoutedEventArgs e)
        {
            User user = new User
            {
                Username = "Anonimas",
                VendorsCount = 0,
                HashedPsw = ""
            };

            MainWindow map = new MainWindow(user);
            map.Show();
            Close();
        }

        private void SignInClicked(object sender, RoutedEventArgs e)
        {
            Login lWindow = new Login();
            lWindow.Show();
            this.Close();
        }

        #endregion
    }
}