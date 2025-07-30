using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using to_do_list.Controllers;
using to_do_list.Data;
using to_do_list.Models;
using Xunit;
using System.Linq;
using to_do_list.Models.ViewModels;
using System;

public class AdminControllerTests
{
    private async Task<ApplicationDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // нова база за всеки тест
            .Options;

        var context = new ApplicationDbContext(options);

        // Създаване на задължителни обекти
        var category = new Category { Id = 1, Name = "TestCat" };
        var priority = new Priority { Id = 1, Name = "High", Color = "#ff0000" }; // 👈 добавено Color
        var user = new ApplicationUser { Id = "user1", UserName = "test@abv.bg", Email = "test@abv.bg" };

        context.Categories.Add(category);
        context.Priorities.Add(priority);
        context.Users.Add(user);

        var task = new TodoTask
        {
            Title = "Test Task",
            UserId = user.Id,
            IsCompleted = false,
            Category = category,
            Priority = priority
        };

        context.TodoTasks.Add(task);

        await context.SaveChangesAsync();
        return context;
    }


    [Fact]
    public async Task Dashboard_ReturnsViewWithModel()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var controller = new AdminController(context);

        // Act
        var result = await controller.Dashboard();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(1, model.TotalUsers);           // 🟢 Ще мине
        Assert.Equal(1, model.TotalTasks);           // 🟢 Ще мине
        Assert.Equal(0, model.CompletedTasks);       // 🟢 Задачата не е изпълнена
        Assert.Equal(1, model.IncompleteTasks);      // 🟢 Има една незавършена
        Assert.Single(model.TasksByCategory);        // 🟢 1 категория
        Assert.Single(model.TasksPerUser);           // 🟢 1 потребител с задача
    }
}
