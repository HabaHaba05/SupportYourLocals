using Microsoft.VisualStudio.PlatformUI;
using SuppLocals.Utilities.Helpers;
using SuppLocals.Views;
using SuppLocals.Views.AccountViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class LoginVM : ObservableObject, IDataErrorInfo
    {
        private string _username;
        private string _password;
        private bool _loginIsDisabled;

      

        public string Error => null;
        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        public string this[string name]
        {
            get
            {
                string result = null;
                var validate = new ValidateUsername();


                switch (name)
                {
                    case "Username":
                        if (string.IsNullOrWhiteSpace(Username))
                        {
                            result = "Username can not be empty!";
                        }
                        else if (Username.Length < 5)
                        {
                            result = "Username has to contain from 5 to 12 symbols!";
                        }

                        break;

                    case "Password":
                        if (string.IsNullOrWhiteSpace(Password))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (Password.Length < 8)
                        {
                            result = "New password has to be at least 8 symbols long!";
                        }
                        else if (validate.IsPasswordValid(Password) is false)
                        {
                            result = validate.PasswordErrorMessage(Password);
                        }

                        break;

                }


                if (ErrorCollection.ContainsKey(name))
                    ErrorCollection[name] = result;
                else if (result != null)
                    ErrorCollection.Add(name, result);

                NotifyPropertyChanged("ErrorCollection");
                return result;
            }
        }


        #region Public props
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

        public bool LoginIsDisabled
        {
            get => _loginIsDisabled;
            set
            {
                _loginIsDisabled = value;
                NotifyPropertyChanged("LoginIsDisabled");
            }
        }

        #endregion
        public RelayCommand LoginClick{ get; }
        

        public LoginVM()
        {
            LoginClick = new RelayCommand(async (x) =>  await LogInBtnClick(), o => ButtonState());
           
        }

        public async Task LogInBtnClick()
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

            if (CloseWindow.WinObject != null)
                CloseWindow.CloseParent();

        }

        private bool ButtonState()
        {
            var passwordValidator = new ValidateUsername();
            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
            {
                return false;
            }
            else if (_password.Length < 8 ||  _username.Length < 5)
            {
                return false;
            }
            else if (passwordValidator.IsPasswordValid(_password) is false )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }


}
