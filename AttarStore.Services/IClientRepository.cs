using AttarStore.Entities;
using AttarStore.Entities.submodels;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public interface IClientRepository
    {
        Task<bool> ExistsByNameOrEmailAsync(string name, string email, int? excludeUserId = null, int? excludeAdminId = null);
        Task<IUser> GetByClientOrEmail(string input);
        Task<Client> GetByClient(string name);
        Task<Client> GetByClientEmail(string email);
        Task<string> GetClientRoleByUsername(string username);
        Task<IUser[]> GetAllClients();
        Task<IUser> GetByRefreshToken(string refreshToken);
        Task AddClient(IUser client);
        Task<Client> GetClientById(int id);
        Task<Client> GetClientByIdToUpdate(int id);
        Task UpdateClientAsync(IUser client);
        Task DeleteClientAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> EmailExists(string email);
        Task<string> UpdateUserProfileAsync(int clientId, string name, string phoneNumber, string email);
        Task<bool> UpdateUserPasswordAsync(int clientId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
