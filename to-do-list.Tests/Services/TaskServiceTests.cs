using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services;
using to_do_list.Services.Interfaces;
using Xunit;

public class TaskServiceTests
{
    private static ApplicationDbContext NewContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task SeedLookupsAsync(ApplicationDbContext ctx)
    {
        if (!await ctx.Categories.AnyAsync())
        {
            ctx.Categories.AddRange(
                new Category { Id = 1, Name = "Work" },
                new Category { Id = 2, Name = "Personal" }
            );
        }
        if (!await ctx.Priorities.AnyAsync())
        {
            ctx.Priorities.AddRange(
                new Priority { Id = 1, Name = "High", Color = "#ff0000" },
                new Priority { Id = 2, Name = "Low", Color = "#00ff00" }
            );
        }
        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAsync_AddsTaskForUser()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = NewContext(dbName);
        await SeedLookupsAsync(ctx);
        ITaskService service = new TaskService(ctx);

        await service.CreateAsync(new TodoTask
        {
            Title = "Task A",
            UserId = "u1",
            DueDate = DateTime.Today,
            CategoryId = 1,
            PriorityId = 1
        });

        Assert.Equal(1, ctx.TodoTasks.Count());
        Assert.Equal("Task A", ctx.TodoTasks.First().Title);
    }

    [Fact]
    public async Task GetUserTasksAsync_FiltersByUser_And_OptionalFilters()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var ctx = NewContext(dbName))
        {
            await SeedLookupsAsync(ctx);

            ctx.TodoTasks.AddRange(
                new TodoTask { Title = "U1-Work-High", UserId = "u1", DueDate = DateTime.Today, CategoryId = 1, PriorityId = 1 },
                new TodoTask { Title = "U1-Personal-Low", UserId = "u1", DueDate = DateTime.Today, CategoryId = 2, PriorityId = 2 },
                new TodoTask { Title = "U2-Work-High", UserId = "u2", DueDate = DateTime.Today, CategoryId = 1, PriorityId = 1 }
            );
            await ctx.SaveChangesAsync();
        }

        using (var ctx = NewContext(dbName))
        {
            ITaskService service = new TaskService(ctx);

            var allU1 = await service.GetUserTasksAsync("u1", null, null, null);
            Assert.Equal(2, allU1.Count);

            var u1Work = await service.GetUserTasksAsync("u1", null, 1, null);
            Assert.Single(u1Work);
            Assert.Equal("U1-Work-High", u1Work.First().Title);

            var u1Low = await service.GetUserTasksAsync("u1", null, null, 2);
            Assert.Single(u1Low);
            Assert.Equal("U1-Personal-Low", u1Low.First().Title);

            var search = await service.GetUserTasksAsync("u1", "Personal", null, null);
            Assert.Single(search);
            Assert.Equal("U1-Personal-Low", search.First().Title);
        }
    }

    [Fact]
    public async Task ToggleStatusAsync_FlipsCompletion()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;

        using (var ctx = NewContext(dbName))
        {
            await SeedLookupsAsync(ctx);
            var task = new TodoTask
            {
                Title = "ToggleMe",
                UserId = "u1",
                DueDate = DateTime.Today,
                CategoryId = 1,
                PriorityId = 1,
                IsCompleted = false
            };
            ctx.TodoTasks.Add(task);
            await ctx.SaveChangesAsync();
            id = task.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ITaskService service = new TaskService(ctx);

            await service.ToggleStatusAsync(id, "u1");
            var after1 = await ctx.TodoTasks.FindAsync(id);
            Assert.True(after1!.IsCompleted);

            await service.ToggleStatusAsync(id, "u1");
            var after2 = await ctx.TodoTasks.FindAsync(id);
            Assert.False(after2!.IsCompleted);
        }
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;

        using (var ctx = NewContext(dbName))
        {
            await SeedLookupsAsync(ctx);
            var t = new TodoTask
            {
                Title = "Old",
                UserId = "u1",
                DueDate = DateTime.Today,
                CategoryId = 1,
                PriorityId = 1
            };
            ctx.TodoTasks.Add(t);
            await ctx.SaveChangesAsync();
            id = t.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ITaskService service = new TaskService(ctx);

            var updated = new TodoTask
            {
                Id = id,
                Title = "Updated",
                UserId = "u1",
                DueDate = DateTime.Today.AddDays(1),
                CategoryId = 2,
                PriorityId = 2,
                IsCompleted = true
            };

            await service.UpdateAsync(updated, "u1");

            var saved = await ctx.TodoTasks.FindAsync(id);
            Assert.Equal("Updated", saved!.Title);
            Assert.Equal(2, saved.CategoryId);
            Assert.Equal(2, saved.PriorityId);
            Assert.True(saved.IsCompleted);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesOnlyOwnerTask()
    {
        var dbName = Guid.NewGuid().ToString();
        int idOwner, idOther;

        using (var ctx = NewContext(dbName))
        {
            await SeedLookupsAsync(ctx);
            var t1 = new TodoTask { Title = "Mine", UserId = "u1", DueDate = DateTime.Today, CategoryId = 1, PriorityId = 1 };
            var t2 = new TodoTask { Title = "NotMine", UserId = "u2", DueDate = DateTime.Today, CategoryId = 1, PriorityId = 1 };
            ctx.TodoTasks.AddRange(t1, t2);
            await ctx.SaveChangesAsync();
            idOwner = t1.Id;
            idOther = t2.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ITaskService service = new TaskService(ctx);

            await service.DeleteAsync(idOwner, "u1");
            Assert.Null(await ctx.TodoTasks.FindAsync(idOwner));

            await service.DeleteAsync(idOther, "u1");
            Assert.NotNull(await ctx.TodoTasks.FindAsync(idOther));
        }
    }
}
