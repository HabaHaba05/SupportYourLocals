
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class ChangeProfileVM : BaseViewModel , IDataErrorInfo
    {
        public static User ActiveUser { get; set; }

        private string _oldpassword;
        private string _newpassword;
        private string _confirmpassword;
        #region Public props
        public string OldPassword
        {
            get => _oldpassword;
            set
            {
                _oldpassword = value;
                NotifyPropertyChanged("OldPassword");
            }
        }
        public string NewPassword
        {
            get => _newpassword;
            set
            {
                _newpassword = value;
                NotifyPropertyChanged("NewPassword");
            }
        }

        public string ConfirmNewPassword
        {
            get => _confirmpassword;
            set
            {
                _confirmpassword = value;
                NotifyPropertyChanged("ConfirmNewPassword");
            }
        }
        #endregion

        private string _test;
        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                NotifyPropertyChanged("OldPassword");
            }
        }

        public ChangeProfileVM(User user)
        {
            ActiveUser = user;
            SaveChangesClick = new RelayCommand(o =>{SaveChanges();}, o => true);
        }

        public RelayCommand SaveChangesClick { get; }


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
                    case "OldPassword":
                        if (string.IsNullOrWhiteSpace(OldPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (OldPassword.Length < 8)
                        {
                            result = "Password has to be at least 8 symbols long!";
                        }
                        break;

                    case "NewPassword":
                        if (string.IsNullOrWhiteSpace(NewPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (NewPassword.Length < 8)
                        {
                            result = "New password has to be at least 8 symbols long!";
                        }
                        else if (validate.IsPasswordValid(NewPassword) is false)
                        {
                            result = validate.PasswordErrorMessage(NewPassword);
                        }
                        break;

                    case "ConfirmNewPassword":
                        if (string.IsNullOrWhiteSpace(ConfirmNewPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (ConfirmNewPassword.Length < 8)
                        {
                            result = "Confirmation password has to be at least 8 symbols long!";
                        }
                        else if (validate.IsPasswordValid(ConfirmNewPassword) is false)
                        {
                            result = validate.PasswordErrorMessage(ConfirmNewPassword);
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
        


        private void SaveChanges()
        {
            if (OldPassword == NewPassword)
            {
                MessageBox.Show("Old and new password can not match!");
                return;
            }
            else if (NewPassword != ConfirmNewPassword)
            {
                Test = "bybiene";
                return;
            }
            else if (!BC.Verify(OldPassword, ActiveUser.HashedPsw))
            {
                MessageBox.Show("Incorrect old password");
                return;
            }


            //changing the current password to new password
            using (var dbUser = new UsersDbTable())
            {
                var user = dbUser.Users.SingleOrDefault(x => x.ID == ActiveUser.ID);
                user.HashedPsw = BC.HashPassword(NewPassword);
                dbUser.SaveChanges();
            }

            NewPassword = "";
            OldPassword = "";
            ConfirmNewPassword = "";
        }



    }
}
