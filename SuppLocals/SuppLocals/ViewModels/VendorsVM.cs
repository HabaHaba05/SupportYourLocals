﻿using System.Collections.Generic;
using System.Linq;

namespace SuppLocals.ViewModels
{
    public class VendorsVM : ObservableObject
    {
        public VendorsVM()
        {
            _userList = new List<User>();

            using (var db = new UsersDbTable())
            {
                var data = db.Users.ToList();
                foreach (var user in data.Where(user => user.VendorsCount > 0))
                {
                    _userList.Add(user);
                }
            }
        }

        private List<User> _userList;
        public List<User> UserList 
        { 
            get => _userList;
            set
            {
                OnPropertyChanged(ref _userList, value);
            }
        }
    }
}