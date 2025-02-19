

namespace AttarStore.Models
{
    public class AdminMapperCreate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; } = "";
    }
    public class AdminMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; } = "";
        public DateTimeOffset Created_at { get; set; } = DateTimeOffset.Now;
        public bool IsDeleted { get; set; } = false;
    }
    public class AdminView
    {
        public string Name { get; set; }

    }
    public class AdminReplyView
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
