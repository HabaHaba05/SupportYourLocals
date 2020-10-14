using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;

namespace SuppLocals
{
    public class ValidateUsername : ObservableObject ,IDataErrorInfo
    {
        public ValidateUsername()
        {
            /* Set default Username and email */
            this.Username = "";
            this.Email = "";
            this.Password = "";
            this.ConfirmPassword = "";
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Error{get { return null; }}

        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "Username":
                        if (string.IsNullOrWhiteSpace(Username))
                            result = "Username can not be empty!";
                        else if (this.Username.Length < 5)
                            result = "Username has to contain from 5 to 12 symbols!";
                        break;
                    case "Email":
                        if (string.IsNullOrWhiteSpace(Email))
                            result = "Email can not be empty!";
                        else if (IsValidEmail(this.Email) == false)
                            result = "Email is not valid!";
                        break;
                    case "Password":
                        if (string.IsNullOrWhiteSpace(Password))
                            result = "Password can not be empty!";
                        else if (this.Password.Length < 8){
                            result = "Password has to be at least 8 symbols long!";
                        }
                        break;
                    case "ConfirmPassword":
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                            result = "Password can not be empty!";
                        else if (this.ConfirmPassword.Length < 8)
                        {
                            result = "Password has to be at least 8 symbols long!";
                        }
                        break;
                }

                if (ErrorCollection.ContainsKey(columnName))
                    ErrorCollection[columnName] = result;
                else if (result != null)
                    ErrorCollection.Add(columnName, result);

                OnPropertyChanged("ErrorCollection");
                return result;
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}