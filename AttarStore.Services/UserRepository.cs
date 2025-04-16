using AttarStore.Entities;
using AttarStore.Entities.submodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // ✅ Check if Name or Email Exists Across Both Users and Admins
        public async Task<bool> ExistsByNameOrEmailAsync(string name, string email, int? excludeUserId = null, int? excludeAdminId = null, int? excludeClientId = null)
        {
            bool clientExists = await _dbContext.Clients
               .AnyAsync(u => (u.Name == name || u.Email == email) && (!excludeClientId.HasValue || u.Id != excludeClientId.Value));

            bool userExists = await _dbContext.Users
                .AnyAsync(u => (u.Name == name || u.Email == email) && !u.IsDeleted && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));

            bool adminExists = await _dbContext.Admins
                .AnyAsync(a => (a.Name == name || a.Email == email) && !a.IsDeleted && (!excludeAdminId.HasValue || a.Id != excludeAdminId.Value));

            return userExists || adminExists;
        }

        // ✅ Get User or Admin by Name or Email
        public async Task<IUser> GetByUserOrEmail(string input)
        {
            var user = await _dbContext.Users
                .Where(u => !u.IsDeleted && u.Role != null)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);

            if (user != null)
            {
                return user;
            }

            var admin = await _dbContext.Admins
                .Where(u => !u.IsDeleted && u.Role != null)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);

            return admin;
        }
        // ✅ Get User by Name
        public async Task<User> GetByUser(string name)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == name && !u.IsDeleted);
        }

        // ✅ Get User by Email
        public async Task<User> GetByUserEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        // ✅ Get Role by Username (Checks both Users and Admins)
        public async Task<string> GetUserRoleByUsername(string username)
        {
            var user = await _dbContext.Users
                .Where(u => !u.IsDeleted && u.Role != null && u.Name == username)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(user)) return user.ToUpper();

            var admin = await _dbContext.Admins
                .Where(a => !a.IsDeleted && a.Name == username)
                .Select(a => a.Role)
                .FirstOrDefaultAsync();

            return admin?.ToUpper() ?? "UNKNOWN";
        }

        // ✅ Get All Users (Excludes Deleted)
        public async Task<IUser[]> GetAllUsers()
        {
            return await _dbContext.Users
                .Where(u => !u.IsDeleted)
                .ToArrayAsync();
        }

        // ✅ Get User by Refresh Token
        public async Task<IUser> GetByRefreshToken(string refreshToken)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        // ✅ Add a New User
        public async Task AddUser(IUser user)
        {
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Get User by ID
        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        // ✅ Get User by ID (for Updates)
        public async Task<User> GetUserByIdToUpdate(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        // ✅ Update a User
        public async Task UpdateUserAsync(IUser user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Soft Delete a User
        public async Task DeleteUserAsync(int id)
        {
            var userToDelete = await _dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                _dbContext.Users.Remove(userToDelete); // Hard delete
                await _dbContext.SaveChangesAsync();
            }
        }


        // ✅ Save Changes to Database
        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync() > 0);
        }

        // ✅ Check if Email Exists
        public async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted)
                || await _dbContext.Admins.AnyAsync(a => a.Email == email && !a.IsDeleted)
                || await _dbContext.Clients.AnyAsync(a => a.Email == email);

        }

        // NEW: Update admin profile (excluding password unless changed)
        public async Task<string> UpdateUserProfileAsync(int userId, string name, string phoneNumber, string email)
        {
            var user = await GetUserById(userId);
            if (user == null) return "User not found";

            bool isUpdated = false;

            // Check if Name is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(name) && name != user.Name)
            {
                bool nameExists = await _dbContext.Admins.AnyAsync(a => a.Id != userId && a.Name == name && !a.IsDeleted) ||
                                  await _dbContext.Users.AnyAsync(u => u.Name == name && !u.IsDeleted) ||
                                  await _dbContext.Clients.AnyAsync(u => u.Name == name);


                if (nameExists)
                    return "Name already exists in the system.";

                user.Name = name;
                isUpdated = true;
            }

            // Check if Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
            {
                bool emailExists = await _dbContext.Admins.AnyAsync(a => a.Id != userId && a.Email == email && !a.IsDeleted) ||
                                   await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted) ||
                                   await _dbContext.Clients.AnyAsync(u => u.Email == email);


                if (emailExists)
                    return "Email already exists in the system.";

                user.Email = email;
                isUpdated = true;
            }

            // Check if Phone is being updated
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != user.Phone)
            {
                user.Phone = phoneNumber;
                isUpdated = true;
            }

            // If no updates were made, return a message indicating no changes
            if (!isUpdated)
                return "No changes have been made.";

            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return "Profile updated successfully.";
        }



        // NEW: Update admin password
        public async Task<bool> UpdateUserPasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await GetUserById(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                return false; // Incorrect current password

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
            if (user == null) return null;

            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return token;
        }

        // Reset Password Using Token
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
            if (user == null || user.ResetToken != token || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return false; // Invalid token or expired
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
