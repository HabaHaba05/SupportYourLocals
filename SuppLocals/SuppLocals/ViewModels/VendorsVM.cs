
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SuppLocals.ViewModels
{
    public class VendorsVM : BaseViewModel
    {
        private List<User> usersList;
        
        public List<User> UserList
        {
            get
            {
                return usersList;
            }
        }

        public VendorsVM()
        {
            usersList = new List<User>();
            using (UsersDbTable db = new UsersDbTable())
            {
                var data = db.Users.ToList();
                foreach(var user in data)
                {
                    if (user.VendorsCount > 0)
                    {
                        usersList.Add(user);
                    }
                }

            }
        }

    }
}
