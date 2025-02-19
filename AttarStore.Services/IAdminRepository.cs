using AttarStore.Entities;
using AttarStore.Entities.submodels;

namespace AttarStore.Services
{
    public interface IAdminRepository
    {
        Task<Admin> GetByAdmin(string username);
        Task<string> GetAdminRoleByUsername(string username);
        Task<Admin[]> GetAllAdmins();
        Task<Admin> GetAdminById(int id);
        Task AddAdmin(Admin admin);
        Task UpdateAdminAsync(Admin admin);
        Task<bool> EmailExists(string email);
    }
}
