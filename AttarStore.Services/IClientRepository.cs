using AttarStore.Entities;
using AttarStore.Entities.submodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{


    public interface IClientRepository
    {

        Task<IUser[]> GetAllClients();
        Task<IUser> GetByClientOrEmail(string input);
        Task<Client> GetClientById(int id);
        Task<Client> GetClientByIdToUpdate(int id);
        Task DeleteClientAsync(int id);
        Task AddClient(IUser client);
        Task UpdateClientAsync(IUser client);
        Task<string> GetClientRoleByClientname(string clientname);
        Task<bool> SaveChangesAsync();
        Task<Client> GetByClient(string name);
        Task<bool> EmailExists(string email);
        Task<IUser> GetByRefreshToken(string refreshToken);
        Task<string> UpdateClientProfileAsync(int clientId, string name, string phoneNumber, string email);
        Task<bool> UpdateClientPasswordAsync(int clientId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<Client> GetByClientEmail(string email);


    }

}
