using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using to_do_list.Models;

namespace to_do_list.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Добавяме контекста
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Стартираме миграция (ако има)
            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 👉 Създаване на ролите, ако не съществуват
            string[] roleNames = { "Администратор", "Потребител" };

            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 👉 Администраторски акаунт
            var adminEmail = "admin@site.bg";
            var adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Администратор");
                }
            }

            // 👉 Обикновен потребител
            var userEmail = "user@site.bg";
            var userPassword = "User123!";

            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(normalUser, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "Потребител");
                }
            }

            // 👉 Добавяне на приоритети, ако няма
            if (!await context.Priorities.AnyAsync())
            {
                await context.Priorities.AddRangeAsync(
                    new Priority { Name = "Висок", Color = "red" },
                    new Priority { Name = "Среден", Color = "orange" },
                    new Priority { Name = "Нисък", Color = "green" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
