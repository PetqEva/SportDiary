using Microsoft.EntityFrameworkCore;
using SportDiary.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SportDiary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<TrainingDiary> TrainingDiaries => Set<TrainingDiary>();
        public DbSet<TrainingEntry> TrainingEntries => Set<TrainingEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1 дневник на ден за потребител
            modelBuilder.Entity<TrainingDiary>()
                .HasIndex(d => new { d.UserProfileId, d.Date })
                .IsUnique();

            modelBuilder.Entity<UserProfile>()
                .HasMany(u => u.TrainingDiaries)
                .WithOne(d => d.UserProfile)
                .HasForeignKey(d => d.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingDiary>()
                .HasMany(d => d.TrainingEntries)
                .WithOne(e => e.TrainingDiary)
                .HasForeignKey(e => e.TrainingDiaryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
