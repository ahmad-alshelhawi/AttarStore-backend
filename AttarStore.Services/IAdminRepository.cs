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
        Task DeleteAdminAsync(Admin admin);
        Task<Admin> GetByAdminEmail(string email);
        Task<string> UpdateAdminProfileAsync(int adminId, string name, string phoneNumber, string email);
        Task<bool> UpdateAdminPasswordAsync(int adminId, string currentPassword, string newPassword);
        Task<IUser> GetByUserOrEmail(string input);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    }
}
