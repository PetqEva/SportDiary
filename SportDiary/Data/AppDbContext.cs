using Microsoft.EntityFrameworkCore;
using SportDiary.Data.Models;

namespace SportDiary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) 
            : base(dbContextOptions) { }

        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public virtual DbSet<TrainingDiary> TrainingDiaries { get; set; } = null!;
        public virtual DbSet<TrainingEntry> TrainingEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
