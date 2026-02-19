using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;

namespace SportDiary.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();

        // ---------- USER + PROFILE SEED ----------
        if (!context.UserProfiles.Any())
        {
            var user = new ApplicationUser
            {
                UserName = "demo@sportdiary.local",
                Email = "demo@sportdiary.local"
            };

            var created = await userManager.CreateAsync(user, "demo123");
            if (created.Succeeded)
            {
                var profile = new UserProfile
                {
                    IdentityUserId = user.Id,
                    Name = "Demo User",
                    Age = 30,
                    Gender = "Other",
                    StartWeightKg = 80,
                    CurrentWeightKg = 80,
                    HeightCm = 170,
                    ActivityLevel = "Medium"
                };

                context.UserProfiles.Add(profile);
                await context.SaveChangesAsync();
            }
        }

        // ---------- EXERCISE SEED ----------
        if (!context.Exercises.Any())
        {
            context.Exercises.AddRange(
                new Exercise { Name = "Bench Press", MuscleGroup = "Chest", Type = Data.Models.Enums.ExerciseType.Strength, Difficulty = Data.Models.Enums.DifficultyLevel.Medium },
                new Exercise { Name = "Squat", MuscleGroup = "Legs", Type = Data.Models.Enums.ExerciseType.Strength, Difficulty = Data.Models.Enums.DifficultyLevel.Hard },
                new Exercise { Name = "Pull Up", MuscleGroup = "Back", Type = Data.Models.Enums.ExerciseType.Strength, Difficulty = Data.Models.Enums.DifficultyLevel.Hard }
            );

            await context.SaveChangesAsync();
        }
    }
}

