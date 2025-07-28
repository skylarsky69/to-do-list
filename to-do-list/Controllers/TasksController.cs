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

        // GET: /Tasks
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

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();

            var tasks = await query.ToListAsync();
            return View(tasks);
        }

        // GET: /Tasks/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            return View();
        }

        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoTask task)
        {
            var user = await _userManager.GetUserAsync(User);
            task.UserId = user.Id;

            if (ModelState.IsValid)
            {
                _context.TodoTasks.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            return View(task);
        }

        // GET: /Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            if (task == null) return NotFound();

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            return View(task);
        }

        // POST: /Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoTask task)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            ViewData["Priorities"] = await _context.Priorities.ToListAsync();
            return View(task);
        }

        // GET: /Tasks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            return View(task);
        }

        // POST: /Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            if (task != null)
            {
                _context.TodoTasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

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
