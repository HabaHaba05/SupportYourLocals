using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SuppLocals.ViewModels
{
    public class VendorsVM : ObservableObject
    {

        ObservableCollection<User> userList;

        #region Public props

        public ObservableCollection<User> UserList
        {
            get { return userList; }
            set
            {
                userList = value;
                NotifyPropertyChanged("UserList");
            }
        }

        #endregion

        #region Commands

        public DelegateCommand<object> SelectedItemChangedCommand { get; set; }

        #endregion

        #region Constructor

        public VendorsVM()
        {
            UserList = new ObservableCollection<User>();
            GetData();
            SelectedItemChangedCommand = new DelegateCommand<object>((selectedItem) => SortList(selectedItem));
        }

        #endregion

        #region Methods

        private void GetData()
        {
            using (var db = new AppDbContext())
            {
                var data = db.Users.ToList();
                foreach (var user in data.Where(i => i.VendorsCount > 0))
                {
                    UserList.Add(user);
                }
            }
        }

        private void SortList(object selectedItem)
        {
            switch (selectedItem)
            {
                case "Vendor A to Z":
                    UserList = new ObservableCollection<User>(UserList.OrderBy(i => i.Username));
                    break;
                case "Vendor Z to A":
                    UserList = new ObservableCollection<User>(UserList.OrderByDescending(i => i.Username));
                    break;
                case "Marketplaces 1 - 9":
                    UserList = new ObservableCollection<User>(UserList.OrderBy(i => i.VendorsCount));
                    break;
                case "Marketplaces 9 - 1":
                    UserList = new ObservableCollection<User>(UserList.OrderByDescending(i => i.VendorsCount));
                    break;
                default:
                    break;

            }
        }

        #endregion
    }
}