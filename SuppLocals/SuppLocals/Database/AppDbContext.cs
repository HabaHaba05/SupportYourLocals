using Microsoft.EntityFrameworkCore;

namespace SuppLocals
{
    public class AppDbContext : DbContext
    {

        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Connection string
            optionsBuilder.UseSqlServer(@"Server=tcp:supporyourlocals.database.windows.net,1433;Initial Catalog=FromLocalsToLocals;Persist Security Info=False;User ID=PSI;Password=koduotojai123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
}
