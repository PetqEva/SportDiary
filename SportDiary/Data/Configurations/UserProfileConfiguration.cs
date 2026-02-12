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
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder
                .HasMany(u => u.TrainingDiaries)
                .WithOne(d => d.UserProfile)
                .HasForeignKey(d => d.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
