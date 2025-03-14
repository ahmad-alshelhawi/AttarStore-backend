using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Pending, Shipped, Delivered, Canceled
        public bool IsDeleted { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
