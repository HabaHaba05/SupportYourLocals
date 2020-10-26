using System;
using System.Collections.Generic;
using System.Text;

namespace SuppLocals.ViewModels
{
    public class SignupVM : ObservableObject
    {
        private string _username;
        private string _password; 
        private string _confirmPassword;
        private string _email;

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





    }
}
