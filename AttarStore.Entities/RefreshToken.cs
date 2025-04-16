using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }

        // Optional foreign keys — only one should be set
        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    }

}
