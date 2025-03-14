using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }  // Foreign Key to Cart
        public Cart Cart { get; set; }  // Navigation Property to Cart

        [Required]
        public int ProductId { get; set; }  // Foreign Key to Product
        public Product Product { get; set; }  // Navigation Property to Product

        [Required]
        public int Quantity { get; set; }
        public decimal Price { get; set; }  // Price at the time of adding to the cart
    }
}
