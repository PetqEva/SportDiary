using Microsoft.EntityFrameworkCore;

namespace SportDiary.Data
{
    public class AppDbContext : DbContext
    {


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
