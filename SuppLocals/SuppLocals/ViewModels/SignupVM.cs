using SuppLocals.Classes;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class SignupVM : ObservableObject, IDataErrorInfo
    {
        private string _username;
        private string _password; 
        private string _confirmPassword;
        private string _email;

        #region Public
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

        public string ConfirmPassword { 
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                NotifyPropertyChanged("ConfirmPassword");
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }
        #endregion

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
                    case "Email":
                        if (string.IsNullOrWhiteSpace(Email))
                        {
                            result = "Email can not be empty!";
                        }
                        else if (Email.IsEmail() == false)
                        {
                            result = "Email is not valid!";
                        }

                        break;

                    case "ConfirmPassword":
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (ConfirmPassword.Length < 8)
                        {
                            result = "Confirmation password has to be at least 8 symbols long!";
                        }
                        else if (validate.IsPasswordValid(ConfirmPassword) is false)
                        {
                            result = validate.PasswordErrorMessage(ConfirmPassword);
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

        public RelayCommand SignupClick { get; }

        public SignupVM()
        {
            SignupClick = new RelayCommand(o => { SignUp_ButtonClick(); }, o => true);
        }

        private void SignUp_ButtonClick()
        {
            var username = _username;
            var password = _password;
            var repeatPassword = _confirmPassword;
            var email = _email;

            if (password != repeatPassword)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            using var db = new AppDbContext();

            var usersList = db.Users.ToList();
            if (usersList.FirstOrDefault(x => x.Username == username) != null || username == "Anonimas")
            {
                MessageBox.Show("Sorry, but username is already taken");
                return;
            }

            var newUser = new User()
            {
                Username = username,
                HashedPsw = BC.HashPassword(password),
                VendorsCount = 0,
                Email = email
            };

            db.Users.Add(newUser);
            db.SaveChanges();


            var mainWindow = new MainWindow(newUser);
            mainWindow.Show();
            
            
        }

    }
}
