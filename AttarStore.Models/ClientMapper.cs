

namespace AttarStore.Models
{
    public class ClientMapperCreate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";
    }
    public class ClientMapperUpdate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";

    }
    public class ClientMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; } = "";

    }
    public class ClientView
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
    public class ClientProfileUpdateMapper
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }


    }
    public class ChangePasswordClient
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }




}
