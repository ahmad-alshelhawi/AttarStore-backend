

namespace AttarStore.Models
{
    public class UserMapperCreate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
    }
    public class UserMapperUpdate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
        public bool IsDeleted { get; set; } = false;

    }
    public class UserMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
        public bool IsDeleted { get; set; } = false;

    }
    public class UserView
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
    public class UserProfileUpdateMapper
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }


    }
    public class ChangePasswordUser
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }




}
