using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services.Interfaces;

namespace to_do_list.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TodoTask>> GetUserTasksAsync(string userId, string? searchString, int? categoryId, int? priorityId)
        {
            var q = _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchString))
                q = q.Where(t => t.Title.Contains(searchString));

            if (categoryId.HasValue)
                q = q.Where(t => t.CategoryId == categoryId.Value);

            if (priorityId.HasValue)
                q = q.Where(t => t.PriorityId == priorityId.Value);

            return await q.ToListAsync();
        }

        public Task<TodoTask?> GetByIdAsync(int id)
            => _context.TodoTasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task CreateAsync(TodoTask task)
        {
            _context.TodoTasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoTask task, string userId)
        {
            // Зареждаме текущата задача на този потребител
            var entity = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == task.Id && t.UserId == userId);

            if (entity == null)
                throw new UnauthorizedAccessException("Task not found or not owned by user.");

            // Обновяваме само позволените полета (UserId не пипаме)
            entity.Title = task.Title;
            entity.Description = task.Description;
            entity.DueDate = task.DueDate;
            entity.CategoryId = task.CategoryId;
            entity.PriorityId = task.PriorityId;
            entity.IsCompleted = task.IsCompleted;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var task = await _context.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return;

            _context.TodoTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id, string userId)
        {
            var task = await _context.TodoTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
                throw new UnauthorizedAccessException("Task not found or not owned by user.");

            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();
        }
    }
}
