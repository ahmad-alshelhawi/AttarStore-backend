using AttarStore.Entities;
using AttarStore.Entities.submodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // ✅ Check if Name or Email Exists Across Users, Admins, and Clients
        public async Task<bool> ExistsByNameOrEmailAsync(string name, string email, int? excludeUserId = null, int? excludeAdminId = null)
        {
            bool userExists = await _dbContext.Users
                .AnyAsync(u => (u.Name == name || u.Email == email) && !u.IsDeleted && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));

            bool adminExists = await _dbContext.Admins
                .AnyAsync(a => (a.Name == name || a.Email == email) && !a.IsDeleted && (!excludeAdminId.HasValue || a.Id != excludeAdminId.Value));

            bool clientExists = await _dbContext.Clients
                .AnyAsync(c => (c.Name == name || c.Email == email) && (!excludeAdminId.HasValue || c.Id != excludeAdminId.Value));

            return userExists || adminExists || clientExists;
        }

        // ✅ Get Client, User, or Admin by Name or Email
        public async Task<IUser> GetByClientOrEmail(string input)
        {
            var client = await _dbContext.Clients
                .Where(c => c.Role != null)
                .SingleOrDefaultAsync(c => c.Name == input || c.Email == input);

            if (client != null) return client;

            var user = await _dbContext.Users
                .Where(u => !u.IsDeleted && u.Role != null)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);

            if (user != null) return user;

            var admin = await _dbContext.Admins
                .Where(a => !a.IsDeleted && a.Role != null)
                .SingleOrDefaultAsync(a => a.Name == input || a.Email == input);

            return admin;
        }

        // ✅ Get Client by Name
        public async Task<Client> GetByClient(string name)
        {
            return await _dbContext.Clients.FirstOrDefaultAsync(c => c.Name == name);
        }

        // ✅ Get Client by Email
        public async Task<Client> GetByClientEmail(string email)
        {
            return await _dbContext.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        // ✅ Get Client Role by Username
        public async Task<string> GetClientRoleByUsername(string username)
        {
            var role = await _dbContext.Clients
                .Where(c => c.Role != null && c.Name == username)
                .Select(c => c.Role)
                .FirstOrDefaultAsync();

            return role?.ToUpper();
        }

        // ✅ Get All Clients
        public async Task<IUser[]> GetAllClients()
        {
            return await _dbContext.Clients.ToArrayAsync();
        }

        // ✅ Get Client by Refresh Token
        public async Task<IUser> GetByRefreshToken(string refreshToken)
        {
            return await _dbContext.Clients
                .FirstOrDefaultAsync(c => c.RefreshToken == refreshToken);
        }

        // ✅ Add New Client
        public async Task AddClient(IUser client)
        {
            _dbContext.Add(client);
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Get Client by ID
        public async Task<Client> GetClientById(int id)
        {
            return await _dbContext.Clients
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Id == id);
        }

        // ✅ Get Client by ID (for updates)
        public async Task<Client> GetClientByIdToUpdate(int id)
        {
            return await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        // ✅ Update Client
        public async Task UpdateClientAsync(IUser client)
        {
            _dbContext.Entry(client).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Soft Delete Client
        public async Task DeleteClientAsync(Client client)
        {
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

        }

        // ✅ Save Changes to Database
        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync() > 0);
        }

        // ✅ Check if Email Exists across Users, Admins, Clients
        public async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted)
                || await _dbContext.Admins.AnyAsync(a => a.Email == email && !a.IsDeleted)
                || await _dbContext.Clients.AnyAsync(c => c.Email == email);
        }

        // ✅ Update Client Profile
        public async Task<string> UpdateUserProfileAsync(int clientId, string name, string phoneNumber, string email)
        {
            var client = await GetClientById(clientId);
            if (client == null) return "Client not found";

            bool isUpdated = false;

            // Name Update & Check
            if (!string.IsNullOrWhiteSpace(name) && name != client.Name)
            {
                bool nameExists = await _dbContext.Clients.AnyAsync(c => c.Id != clientId && c.Name == name) ||
                                  await _dbContext.Users.AnyAsync(u => u.Name == name && !u.IsDeleted) ||
                                  await _dbContext.Admins.AnyAsync(a => a.Name == name && !a.IsDeleted);

                if (nameExists)
                    return "Name already exists in the system.";

                client.Name = name;
                isUpdated = true;
            }

            // Email Update & Check
            if (!string.IsNullOrWhiteSpace(email) && email != client.Email)
            {
                bool emailExists = await _dbContext.Clients.AnyAsync(c => c.Id != clientId && c.Email == email) ||
                                   await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted) ||
                                   await _dbContext.Admins.AnyAsync(a => a.Email == email && !a.IsDeleted);

                if (emailExists)
                    return "Email already exists in the system.";

                client.Email = email;
                isUpdated = true;
            }

            // Phone Update
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != client.Phone)
            {
                client.Phone = phoneNumber;
                isUpdated = true;
            }

            if (!isUpdated)
                return "No changes have been made.";

            _dbContext.Entry(client).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return "Profile updated successfully.";
        }
        public async Task<bool> UpdateUserPasswordAsync(int clientId, string currentPassword, string newPassword)
        {
            var client = await GetClientById(clientId);
            if (client == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, client.Password))
                return false; // Incorrect current password

            client.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _dbContext.Entry(client).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(a => a.Email == email);
            if (client == null)
                return null;

            // Generate token using some method (e.g., GUID, JWT token, etc.)
            var token = Guid.NewGuid().ToString();

            // Save token to the database or some temporary storage
            // Optionally, you can expire the token after some time or add more security checks
            client.ResetToken = token;
            client.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Expire in 1 hour
            await _dbContext.SaveChangesAsync();

            return token;
        }

        // Reset password for Admin
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(a => a.Email == email && a.ResetToken == token);
            if (client == null || client.ResetTokenExpiry < DateTime.UtcNow)
                return false; // Token is either invalid or expired

            // Hash the new password (assumes you're using a hash function like BCrypt or similar)
            client.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            client.ResetToken = null; // Invalidate the token after use
            client.ResetTokenExpiry = null; // Clear the expiry date
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
