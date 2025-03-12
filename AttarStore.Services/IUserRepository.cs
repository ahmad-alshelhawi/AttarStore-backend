
using AttarStore.Entities;
using AttarStore.Entities.submodels;

namespace AttarStore.Services
{

    public interface IUserRepository
    {

        Task<IUser[]> GetAllUsers();
        Task<IUser> GetByUserOrEmail(string input);
        Task<User> GetUserById(int id);
        Task<User> GetUserByIdToUpdate(int id);
        Task DeleteUserAsync(int id);
        Task AddUser(IUser user);
        Task UpdateUserAsync(IUser user);
        Task<string> GetUserRoleByUsername(string username);
        Task<bool> SaveChangesAsync();
        Task<User> GetByUser(string name);
        Task<bool> EmailExists(string email);
        Task<IUser> GetByRefreshToken(string refreshToken);
        Task<string> UpdateUserProfileAsync(int userId, string name, string phoneNumber, string email);
        Task<bool> UpdateUserPasswordAsync(int userId, string currentPassword, string newPassword);

    }

}
