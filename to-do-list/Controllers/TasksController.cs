using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;

namespace to_do_list.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task LoadViewDataAsync(object? selectedCategory = null)
        {
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            ViewData["SelectedCategory"] = selectedCategory;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, int? priorityId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var query = _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Where(t => t.UserId == currentUserId);

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(t => t.Title.Contains(searchString));

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (priorityId.HasValue)
                query = query.Where(t => t.PriorityId == priorityId.Value);

            await LoadViewDataAsync(categoryId);
            var tasks = await query.ToListAsync();
            return View(tasks);
        }

        public async Task<IActionResult> Create(int? categoryId = null)
        {
            await LoadViewDataAsync(categoryId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoTask task)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            task.UserId = user.Id;

            if (ModelState.IsValid)
            {
                _context.TodoTasks.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadViewDataAsync(task.CategoryId);
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if (task == null || task.UserId != currentUserId)
                return Unauthorized();

            await LoadViewDataAsync(task.CategoryId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoTask task)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (id != task.Id)
                return NotFound();

            var existingTask = await _context.TodoTasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existingTask == null || existingTask.UserId != currentUserId)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                task.UserId = currentUserId;
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadViewDataAsync(task.CategoryId);
            return View(task);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null || task.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            if (task != null && task.UserId == _userManager.GetUserId(User))
            {
                _context.TodoTasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null || task.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            if (task == null || task.UserId != _userManager.GetUserId(User))
                return NotFound();

            task.IsCompleted = !task.IsCompleted;
            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
