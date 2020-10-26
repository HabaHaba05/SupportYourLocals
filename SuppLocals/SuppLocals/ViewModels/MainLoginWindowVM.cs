using SuppLocals.Views;
using SuppLocals.Views.AccountViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class MainLoginWindowVM : ObservableObject
    {
        private ICommand _gotoToMap;
        private ICommand _gotoSignupCommand;
        private ICommand _loginUserCommand;

        private object _currentView;
        private object _loginView;
        private object _signupView;

        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyPropertyChanged("Username");
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
            }
        }

        public MainLoginWindowVM()
        {
            _loginView = new LoginView();
            _signupView = new SignupView();

            CurrentView = _loginView;
        }


        public ICommand GotoSignupCommand
        {
            get
            {
                return _gotoSignupCommand ??= new RelayCommand(
                   x =>
                   {
                        GotoSignup();
                   });
            }
        }
        public ICommand GoToMap
        {
            get
            {
                return _gotoToMap ??= new RelayCommand(
                   x =>
                   {
                       GoToMapClick();
                   });
            }
        }


        public ICommand LoginUserCommand
        {
            get
            {
                return _loginUserCommand ?? (_loginUserCommand = new RelayCommand(
                   x =>
                   {
                       Login();
                   }));
            }
        }


        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                NotifyPropertyChanged("CurrentView");
            }
        }

        private void GotoView1()
        {
            CurrentView = _loginView;
        }

        private void GotoSignup()
        {
            CurrentView = _signupView;
        }

        public void Login()
        {
            using var db = new AppDbContext();
            var username = _username;
            var password = _password;

            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => (x.Username == username) & BC.Verify(password, x.HashedPsw));
            if (user == null)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            var map = new MainWindow(user);
            map.Show();
             
        }

        private void GoToMapClick()
        {
            var user = new User
            {
                Username = "Anonimas",
                VendorsCount = 0,
                HashedPsw = ""
            };

            var map = new MainWindow(user);
            map.Show();

        }

    }
}
