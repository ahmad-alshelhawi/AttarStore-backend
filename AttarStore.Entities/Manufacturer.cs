using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string? Address { get; set; }
        public bool IsDeleted { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

    }
}
