using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Text.RegularExpressions;

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
                        else if (IsValidEmail(Email) == false)
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

        public bool IsValidEmail(string email)
        {
            
                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}