using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class InventoryProduct
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }  // Foreign key to Inventory
        public int ProductId { get; set; }    // Foreign key to Product

        public int Quantity { get; set; }  // Quantity in the inventory
        public decimal Price { get; set; } // Price of the product in this inventory

        public Inventory Inventory { get; set; }  // Navigation property to Inventory
        public Product Product { get; set; }      // Navigation property to Product
    }
}
