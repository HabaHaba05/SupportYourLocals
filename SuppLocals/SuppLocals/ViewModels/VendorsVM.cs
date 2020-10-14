using System.Collections.Generic;
using System.Linq;

namespace SuppLocals.ViewModels
{
    public class VendorsVM : BaseViewModel
    {
        public VendorsVM()
        {
            UserList = new List<User>();

            using (var db = new UsersDbTable())
            {
                var data = db.Users.ToList();
                foreach (var user in data.Where(user => user.VendorsCount > 0))
                {
                    UserList.Add(user);
                }
            }
        }

        public List<User> UserList { get; }
    }
}