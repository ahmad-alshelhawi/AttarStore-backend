
using AttarStore.Entities.submodels;

namespace AttarStore.Entities
{
    public class Admin : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } = "Master";

        public string Address { get; set; } = "";
        public DateTimeOffset Created_at { get; set; } = DateTimeOffset.Now;
        public bool IsDeleted { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}
