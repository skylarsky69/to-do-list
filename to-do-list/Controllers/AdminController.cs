using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace to_do_list.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var users = await _context.Users.ToListAsync();

            var tasks = await _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .ToListAsync();

            var tasksByCategory = tasks
                .GroupBy(t => t.Category?.Name ?? "Без категория")
                .Select(g => new CategoryStats
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var tasksPerUser = tasks
                .Where(t => t.UserId != null)
                .GroupBy(t => t.UserId)
                .Select(g => new UserStats
                {
                    UserId = g.Key!,
                    Count = g.Count()
                })
                .ToList();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.IsCompleted),
                IncompleteTasks = tasks.Count(t => !t.IsCompleted),
                TasksByCategory = tasksByCategory,
                TasksPerUser = tasksPerUser,
                Users = users
            };

            return View(viewModel);
        }
    }
}
