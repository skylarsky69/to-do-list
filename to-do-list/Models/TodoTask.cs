using System.ComponentModel.DataAnnotations;

namespace to_do_list.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        public bool IsCompleted { get; set; }

        public string? UserId { get; set; }

        public Category? Category { get; set; }
        public Priority? Priority { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
