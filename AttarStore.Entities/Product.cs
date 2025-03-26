using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AttarStore.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // URL of the category image

        // Foreign key for Category
        public int CategoryId { get; set; }

        [JsonIgnore] // Prevents looping
        public Category Category { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public List<InventoryProduct> InventoryProducts { get; set; }
        public List<CartItem> CartItems { get; set; }

    }
}
