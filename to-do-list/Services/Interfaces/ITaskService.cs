using to_do_list.Models;

namespace to_do_list.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TodoTask>> GetUserTasksAsync(string userId, string? searchString, int? categoryId, int? priorityId);
        Task<TodoTask?> GetByIdAsync(int id);
        Task CreateAsync(TodoTask task);
        Task UpdateAsync(TodoTask task, string userId);
        Task DeleteAsync(int id, string userId);
        Task ToggleStatusAsync(int id, string userId);
    }
}
