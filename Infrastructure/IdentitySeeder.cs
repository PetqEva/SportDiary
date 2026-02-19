using Microsoft.AspNetCore.Identity;
using SportDiary.Data.Models;
using SportDiary.GCommon;

namespace SportDiary.Infrastructure
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Roles (изискване: User + Administrator)
            if (!await roleManager.RoleExistsAsync(Roles.User))
                await roleManager.CreateAsync(new IdentityRole(Roles.User));

            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));

            // 2) Премахване на старата "Admin" роля (ако съществува)
            if (await roleManager.RoleExistsAsync("Admin"))
            {
                var oldRole = await roleManager.FindByNameAsync("Admin");
                if (oldRole != null)
                    await roleManager.DeleteAsync(oldRole);
            }

            // 3) Admin user
            var adminEmail = "admin@sportdiary.bg";
            var adminPassword = "Admin123!";

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
                    return;
            }

            // 4) Добавяне към Administrator
            if (!await userManager.IsInRoleAsync(admin, Roles.Administrator))
                await userManager.AddToRoleAsync(admin, Roles.Administrator);
        }
    }
}
