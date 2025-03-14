using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }  // Example: Warehouse location

        public bool IsDeleted { get; set; }

        public List<InventoryProduct> InventoryProducts { get; set; }  // Navigation property to InventoryProduct
    }
}
