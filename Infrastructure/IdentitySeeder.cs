using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SportDiary.Data.Models;

namespace SportDiary.Infrastructure
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";

        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Roles
            if (!await roleManager.RoleExistsAsync(AdminRole))
                await roleManager.CreateAsync(new IdentityRole(AdminRole));

            // 2) Admin user (смени имейла/паролата по твой избор)
            var adminEmail = "admin@sportdiary.bg";
            var adminPassword = "Admin123!"; // за предаване може да е по-лесна, после я стягаш

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    // ако искаш, логни грешките; засега просто излизаме
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
                await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
