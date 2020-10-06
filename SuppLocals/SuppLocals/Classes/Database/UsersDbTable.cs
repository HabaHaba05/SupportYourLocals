using Microsoft.EntityFrameworkCore;

namespace SuppLocals
{
    public class UsersDbTable : AppDbContext
    {
        public DbSet<User> Users { get; set; }

    }
}
