using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using to_do_list.Controllers;
using to_do_list.Data;
using to_do_list.Models;
using Xunit;
using System.Linq;

// НОВО:
using to_do_list.Services;
using to_do_list.Services.Interfaces;

public class TasksControllerTests
{
    private static UserManager<ApplicationUser> MockUserManagerReturning(string userId)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        mgr.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        return mgr.Object;
    }

    [Fact]
    public async Task Index_ReturnsViewWithUserTasks()
    {
        var dbName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var testUserId = "user123";

        // Seed
        using (var context = new ApplicationDbContext(options))
        {
            var category = new Category { Id = 1, Name = "Тест Категория" };
            var priority = new Priority { Id = 1, Name = "Висок", Color = "#ff0000" };

            context.Categories.Add(category);
            context.Priorities.Add(priority);

            context.TodoTasks.Add(new TodoTask
            {
                Title = "Тест задача",
                UserId = testUserId,
                DueDate = DateTime.Today,
                CategoryId = category.Id,
                PriorityId = priority.Id
            });

            await context.SaveChangesAsync();
        }

        // Act (нов контекст + реална TaskService)
        using (var context = new ApplicationDbContext(options))
        {
            var taskService = new TaskService(context);
            var userManager = MockUserManagerReturning(testUserId);

            // Забележка: използваме конструктора без DbContext, защото ViewData списъците не са цел на този тест
            var controller = new TasksController(taskService, userManager);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TodoTask>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Тест задача", model.First().Title);
        }
    }
}
