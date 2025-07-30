namespace to_do_list.Models
{
    public class AdminStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int IncompleteTasks { get; set; }
        public List<CategoryStat> TasksByCategory { get; set; }
        public List<UserTaskStat> TasksPerUser { get; set; }
    }

    public class CategoryStat
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }

    public class UserTaskStat
    {
        public string UserId { get; set; }
        public int Count { get; set; }
    }
}
