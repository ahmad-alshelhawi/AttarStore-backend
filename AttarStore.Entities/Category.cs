using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string? ImageUrl { get; set; } // URL of the category image

        // Navigation property for self-referencing parent category
        public Category? Parent { get; set; }

        // Navigation properties
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
