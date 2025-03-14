using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign Key to Client
        public User User { get; set; }  // Navigation Property to Client

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }


}
