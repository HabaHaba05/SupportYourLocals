using Microsoft.EntityFrameworkCore;

namespace SuppLocals
{
    public class VendorsDbTable : AppDbContext  
    {
        public DbSet<Vendor> Vendors { get; set; }
    }
}
