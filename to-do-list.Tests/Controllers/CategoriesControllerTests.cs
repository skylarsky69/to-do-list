using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using to_do_list.Controllers;
using to_do_list.Data;
using to_do_list.Models;
using Xunit;
using System.Linq;
using System.Collections.Generic;

public class CategoriesControllerTests
{
    private async Task<ApplicationDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("CategoriesTestDb")
            .Options;

        var context = new ApplicationDbContext(options);

        context.Categories.Add(new Category { Id = 1, Name = "Work" });
        context.Categories.Add(new Category { Id = 2, Name = "Personal" });

        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task Index_ReturnsAllCategories()
    {
        var context = await GetInMemoryDbContextAsync();
        var controller = new CategoriesController(context);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);

        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Create_AddsCategory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("CreateCategoryTestDb")
            .Options;

        using var context = new ApplicationDbContext(options);
        var controller = new CategoriesController(context);

        var newCategory = new Category { Name = "NewCategory" };
        var result = await controller.Create(newCategory);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(1, context.Categories.Count());
        Assert.Equal("NewCategory", context.Categories.First().Name);
    }
}
