﻿
using AttarStore.Entities.submodels;

namespace AttarStore.Entities
{
    public class Client : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } = "Client";
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string Address { get; set; } = "";
        public DateTimeOffset Created_at { get; set; } = DateTimeOffset.Now;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public List<PermissionUser> PermissionUsers { get; set; }
        public List<Order> Orders { get; set; }
        //public List<RefreshToken> RefreshTokens { get; set; }
        public List<ActionLog> ActionLogs { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }

    }
}
