using Microsoft.Win32;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using System.Windows.Media;
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

        public ChangeProfileVM(User user)
        {
            ActiveUser = user;

            SaveChangesClick = new RelayCommand(o => { SaveChanges(); }, o => true);
        }

        public RelayCommand SaveChangesClick { get; }

        public string Error => null;
        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();
        public string this[string name]
        {
            get
            {
                string result = null;

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
                        if (string.IsNullOrWhiteSpace(OldPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (NewPassword.Length < 8)
                        {
                            result = "New password has to be at least 8 symbols long!";
                        }
                        break;

                    case "ConfirmNewPassword":
                        if (string.IsNullOrWhiteSpace(OldPassword))
                        {
                            result = "Field cannot be empty";
                        }
                        else if (ConfirmNewPassword.Length < 8)
                        {
                            result = "Confirmation password has to be at least 8 symbols long!";

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
                MessageBox.Show("New password do not match confirm password!");
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
