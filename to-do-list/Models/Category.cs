using System.ComponentModel.DataAnnotations;

namespace to_do_list.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името на категорията е задължително.")]
        [StringLength(50, ErrorMessage = "Името не може да надвишава 50 символа.")]
        public string Name { get; set; }
    }
}
