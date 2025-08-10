using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services.Interfaces;

namespace to_do_list.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _tasks;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext? _context; // може да е null в тестове

        // Продукционен конструктор (DI от Program.cs)
        public TasksController(ITaskService tasks, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _tasks = tasks;
            _userManager = userManager;
            _context = context;
        }

        // За unit тестове (ако не подавате DbContext)
        public TasksController(ITaskService tasks, UserManager<ApplicationUser> userManager)
        {
            _tasks = tasks;
            _userManager = userManager;
        }

        private async Task LoadListsAsync(object? selectedCategory = null)
        {
            // Зарежда списъци за dropdown-ите във View-то (ако имаме DbContext)
            if (_context == null) return;

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            ViewData["SelectedCategory"] = selectedCategory;
        }

        // GET: /Tasks
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? priorityId)
        {
            var userId = _userManager.GetUserId(User);
            var model = await _tasks.GetUserTasksAsync(userId, searchString, categoryId, priorityId);
            await LoadListsAsync(categoryId);
            return View(model);
        }

        // GET: /Tasks/Create
        public async Task<IActionResult> Create(int? categoryId = null)
        {
            await LoadListsAsync(categoryId);
            return View();
        }

        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoTask task)
        {
            var userId = _userManager.GetUserId(User);
            task.UserId = userId;

            if (!ModelState.IsValid)
            {
                await LoadListsAsync(task.CategoryId);
                return View(task);
            }

            await _tasks.CreateAsync(task);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _tasks.GetByIdAsync(id);
            var userId = _userManager.GetUserId(User);

            if (task == null || task.UserId != userId) return Unauthorized();

            await LoadListsAsync(task.CategoryId);
            return View(task);
        }

        // POST: /Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoTask task)
        {
            var userId = _userManager.GetUserId(User);
            if (id != task.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadListsAsync(task.CategoryId);
                return View(task);
            }

            await _tasks.UpdateAsync(task, userId);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _tasks.GetByIdAsync(id);
            var userId = _userManager.GetUserId(User);

            if (task == null || task.UserId != userId) return NotFound();

            return View(task);
        }

        // POST: /Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _tasks.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _tasks.GetByIdAsync(id);
            var userId = _userManager.GetUserId(User);

            if (task == null || task.UserId != userId) return NotFound();

            return View(task);
        }

        // POST: /Tasks/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _tasks.ToggleStatusAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
