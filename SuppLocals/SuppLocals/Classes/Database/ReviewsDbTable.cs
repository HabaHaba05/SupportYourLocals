using Microsoft.EntityFrameworkCore;

namespace SuppLocals
{
    public class ReviewsDbTable:AppDbContext
    { 
        public DbSet<Review> Reviews { get; set; }
    }
}
