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

        if (context.UserProfiles.Any())
            return;

        var user = new ApplicationUser
        {
            UserName = "demo@sportdiary.local",
            Email = "demo@sportdiary.local"
        };

        var created = await userManager.CreateAsync(user, "demo123");
        if (!created.Succeeded) return;

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

