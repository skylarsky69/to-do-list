using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services;
using to_do_list.Services.Interfaces;
using Xunit;

public class CategoryServiceTests
{
    private static ApplicationDbContext NewContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeededItems()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var ctx = NewContext(dbName))
        {
            ctx.Categories.AddRange(
                new Category { Name = "Work" },
                new Category { Name = "Personal" }
            );
            await ctx.SaveChangesAsync();
        }

        using (var ctx = NewContext(dbName))
        {
            ICategoryService service = new CategoryService(ctx);
            var all = await service.GetAllAsync();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, c => c.Name == "Work");
            Assert.Contains(all, c => c.Name == "Personal");
        }
    }

    [Fact]
    public async Task CreateAsync_AddsCategory()
    {
        var dbName = Guid.NewGuid().ToString();
        using var ctx = NewContext(dbName);
        ICategoryService service = new CategoryService(ctx);

        await service.CreateAsync(new Category { Name = "NewCat" });

        Assert.Equal(1, ctx.Categories.Count());
        Assert.Equal("NewCat", ctx.Categories.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsItem()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = NewContext(dbName))
        {
            var c = new Category { Name = "Edu" };
            ctx.Categories.Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ICategoryService service = new CategoryService(ctx);
            var found = await service.GetByIdAsync(id);

            Assert.NotNull(found);
            Assert.Equal("Edu", found!.Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_ModifiesName()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = NewContext(dbName))
        {
            var c = new Category { Name = "Old" };
            ctx.Categories.Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ICategoryService service = new CategoryService(ctx);
            await service.UpdateAsync(new Category { Id = id, Name = "Updated" });

            var updated = await ctx.Categories.FindAsync(id);
            Assert.Equal("Updated", updated!.Name);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        var dbName = Guid.NewGuid().ToString();
        int id;
        using (var ctx = NewContext(dbName))
        {
            var c = new Category { Name = "Temp" };
            ctx.Categories.Add(c);
            await ctx.SaveChangesAsync();
            id = c.Id;
        }

        using (var ctx = NewContext(dbName))
        {
            ICategoryService service = new CategoryService(ctx);
            await service.DeleteAsync(id);

            Assert.Null(await ctx.Categories.FindAsync(id));
        }
    }
}
