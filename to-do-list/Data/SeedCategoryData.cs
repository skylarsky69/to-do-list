using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using to_do_list.Models;

namespace to_do_list.Data
{
    public static class SeedCategoryData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Categories.Any())
                return;

            context.Categories.AddRange(
                new Category { Name = "Работа" },
                new Category { Name = "Лични" },
                new Category { Name = "Здраве" },
                new Category { Name = "Образование" },
                new Category { Name = "Семейство" },
                new Category { Name = "Пътувания" },
                new Category { Name = "Финанси" },
                new Category { Name = "Проекти" },
                new Category { Name = "Хобита" },
                new Category { Name = "Домашни любимци" },
                new Category { Name = "Покупки" },
                new Category { Name = "Социални ангажименти" },
                new Category { Name = "Подобрение на умения" },
                new Category { Name = "Къщна поддръжка" }
            );

            context.SaveChanges();
        }
    }
}
