using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportDiary.Data.Models;
using static SportDiary.Common.ValidationConstants;

namespace SportDiary.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.IdentityUserId)
                .IsRequired();

            builder.HasIndex(p => p.IdentityUserId)
                .IsUnique();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            // ✅ 1 към 1: IdentityUser <-> UserProfile
            builder.HasOne(p => p.IdentityUser)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(p => p.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.TrainingDiaries)
                .WithOne(d => d.UserProfile)
                .HasForeignKey(d => d.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

