

using System.ComponentModel.DataAnnotations;

namespace AttarStore.Models
{
    public class AdminMapperCreate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
    }
    public class AdminMapperUpdate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
        public bool IsDeleted { get; set; } = false;

    }
    public class AdminMapperView
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
    public class AdminView
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
    public class AdminProfileUpdateMapper
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }


    }
    public class ChangePasswordAdmin
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }



}
