using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data.Configurations;
using SportDiary.Data.Models;

namespace SportDiary.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;

        public virtual DbSet<TrainingDiary> TrainingDiaries { get; set; } = null!;

        public virtual DbSet<TrainingEntry> TrainingEntries { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingDiaryConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingEntryConfiguration());
        }
    }
}
