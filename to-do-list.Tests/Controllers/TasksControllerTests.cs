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

public class TasksControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewWithUserTasks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // нова база за всеки тест
            .Options;

        var testUserId = "user123";

        using (var context = new ApplicationDbContext(options))
        {
            // Създаване на задължителни Category и Priority
            var category = new Category { Id = 1, Name = "Тест Категория" };
            var priority = new Priority { Id = 1, Name = "Висок", Color = "#ff0000" };

            context.Categories.Add(category);
            context.Priorities.Add(priority);

            // Добавяне на задача
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

        using (var context = new ApplicationDbContext(options))
        {
            // Mock UserManager
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
            );

            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(testUserId);

            // Създаване на контролера
            var controller = new TasksController(context, userManagerMock.Object);

            // Задаване на User в ControllerContext
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.Index(null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TodoTask>>(viewResult.Model);

            Assert.Single(model); // Очакваме точно една задача
            Assert.Equal("Тест задача", model.First().Title);
        }
    }
}
