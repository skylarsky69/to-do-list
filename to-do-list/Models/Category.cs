namespace to_do_list.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TodoTask> Tasks { get; set; }
    }
}
