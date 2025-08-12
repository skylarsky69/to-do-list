using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using to_do_list.Models;
using to_do_list.Services.Interfaces;

namespace to_do_list.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categories;

        public CategoriesController(ICategoryService categories)
        {
            _categories = categories;
        }

      
        public async Task<IActionResult> Index()
        {
            var model = await _categories.GetAllAsync();
            return View(model);
        }

        public IActionResult Create() => View();

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            await _categories.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categories.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            if (!ModelState.IsValid) return View(category);

            await _categories.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _categories.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
