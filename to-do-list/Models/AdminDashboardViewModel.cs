using System.Collections.Generic;

namespace to_do_list.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int IncompleteTasks { get; set; }

        public List<CategoryStats> TasksByCategory { get; set; }
        public List<UserStats> TasksPerUser { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }

    public class CategoryStats
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }

    public class UserStats
    {
        public string UserId { get; set; }
        public int Count { get; set; }
    }
}
