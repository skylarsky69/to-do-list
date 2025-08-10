using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using to_do_list.Controllers;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services;

public class TasksControllerTests
{
    private static UserManager<ApplicationUser> MockUserManagerReturning(string userId)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        return mgr.Object;
    }

    [Xunit.Fact]
    public async Task Index_ReturnsViewWithUserTasks()
    {
        var dbName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var testUserId = "user123";

        // seed
        using (var seedCtx = new ApplicationDbContext(options))
        {
            var category = new Category { Id = 1, Name = "Тест Категория" };
            var priority = new Priority { Id = 1, Name = "Висок", Color = "#ff0000" };

            seedCtx.Categories.Add(category);
            seedCtx.Priorities.Add(priority);

            seedCtx.TodoTasks.Add(new TodoTask
            {
                Title = "Тест задача",
                UserId = testUserId,
                DueDate = DateTime.Today,
                CategoryId = category.Id,
                PriorityId = priority.Id
            });

            await seedCtx.SaveChangesAsync();
        }

        // run test
        using (var context = new ApplicationDbContext(options))
        {
            var taskService = new TaskService(context);
            var userManager = MockUserManagerReturning(testUserId);

            // 👉 подаваме и context към конструктора
            var controller = new TasksController(taskService, userManager, context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.Index(null, null, null);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<TodoTask>>(viewResult.Model);

            Xunit.Assert.Single(model);
            Xunit.Assert.Equal("Тест задача", model.First().Title);
        }
    }
}
