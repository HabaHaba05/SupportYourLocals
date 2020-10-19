using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.ViewModels
{
    public class ChangeProfileVM : BaseViewModel , IDataErrorInfo
    {
        public User ActiveUser;
        

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
            

            SaveChangesClick = new RelayCommand(o =>
            {
                SaveChanges();
            },
                o => true
            );

            BackButtonClick = new RelayCommand(o =>
            {
                MessageBox.Show(_confirmpassword);
            }
                );
        
        }

        public RelayCommand SaveChangesClick { get; }
        public RelayCommand BackButtonClick { get; }


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
                            result = "Field cannot be empty";
                        break;

                    case "NewPassword":
                        if (string.IsNullOrWhiteSpace(OldPassword))
                            result = "Field cannot be empty";
                        break;

                    case "ConfirmNewPassword":
                        if (string.IsNullOrWhiteSpace(OldPassword))
                            result = "Field cannot be empty";
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
