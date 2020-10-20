using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SuppLocals.Classes;

namespace SuppLocals
{
    public class ValidateUsername : ObservableObject, IDataErrorInfo
    {
        public ValidateUsername()
        {
            /* Set default Username and email */
            Username = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public Dictionary<string, string> ErrorCollection { get; } = new Dictionary<string, string>();

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
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
                    case "Password":
                        if (string.IsNullOrWhiteSpace(Password))
                        {
                            result = "Password can not be empty!";
                        }
                        else if (Password.Length < 8)
                        {
                            result = "Password has to be at least 8 symbols long!";
                        }
                        else if(IsPasswordValid(Password) is false)
                        {
                            result = PasswordErrorMessage(Password);
                        }

                        break;
                    case "ConfirmPassword":
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                        {
                            result = "Password can not be empty!";
                        }
                        else if (ConfirmPassword.Length < 8)
                        {
                            result = "Password has to be at least 8 symbols long!";
                        }
                        else if (IsPasswordValid(Password) is false)
                        {
                            result = PasswordErrorMessage(Password);
                        }

                        break;
                }

                if (ErrorCollection.ContainsKey(columnName))
                {
                    ErrorCollection[columnName] = result;
                }
                else if (result != null)
                {
                    ErrorCollection.Add(columnName, result);
                }

                OnPropertyChanged("ErrorCollection");
                return result;
            }
        }

        public bool IsPasswordValid(string input) 
        {
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasNumber = new Regex(@"[0-9]+");

            if (!hasLowerChar.IsMatch(input))
            {
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                return false;
            }

            else { return true; }
        }

        public string PasswordErrorMessage(string input) 
        {
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasNumber = new Regex(@"[0-9]+");

            if (!hasLowerChar.IsMatch(input))
            {
                return "Password has to contain 1 lowercase letter!";
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                return "Password has to contain 1 uppercase letter!";
            }
            else if (!hasNumber.IsMatch(input))
            {
                return "Password has to contain 1 number!";
            }
            else
            {
                return "Upsy";
            }

        }


    }
}