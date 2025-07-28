namespace to_do_list.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int PriorityId { get; set; }
        public Priority Priority { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public bool IsCompleted { get; set; } = false;



    }
}
