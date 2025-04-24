using System;

namespace AttarStore.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }          // “Master”, “Admin”, “User” or “Client”
        public int? UserId { get; set; }
        public int? AdminId { get; set; }
        public int? ClientId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
