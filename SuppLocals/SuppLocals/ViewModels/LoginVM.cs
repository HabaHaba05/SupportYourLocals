using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class LoginVM : ObservableObject
    {
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

        public RelayCommand LoginClick{ get; }

        public LoginVM()
        {
            LoginClick = new RelayCommand(o => { LogInBtnClick(); }, o => true);
        }

        public void LogInBtnClick()
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

    }
}
