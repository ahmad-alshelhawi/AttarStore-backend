using AttarStore.Entities;
using AttarStore.Entities.submodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _dbContext;

        public AdminRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Admin> GetByAdmin(string username)
        {
            return await _dbContext.Admins
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync(a => a.Name == username);
        }

        public async Task<string> GetAdminRoleByUsername(string username)
        {
            var admin = await _dbContext.Admins
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync(a => a.Name == username);

            return admin?.Role?.ToUpper();
        }

        public async Task<Admin[]> GetAllAdmins()
        {
            return await _dbContext.Admins
                .Where(u => !u.IsDeleted)
                .ToArrayAsync();
        }
        public async Task<IUser> GetByUserOrEmail(string input)
        {

            var admin = await _dbContext.Admins
                .Where(u => !u.IsDeleted && u.Role != null)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);

            return admin;
        }
        public async Task<Admin> GetAdminById(int id)
        {
            return await _dbContext.Admins
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task AddAdmin(Admin admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            // Check if the name or email already exists in Admins or Users tables
            var nameExists = await _dbContext.Admins.AnyAsync(a => a.Name == admin.Name && !a.IsDeleted) ||
                             await _dbContext.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted) ||
                             await _dbContext.Clients.AnyAsync(u => u.Name == admin.Name);


            var emailExists = await _dbContext.Admins.AnyAsync(a => a.Email == admin.Email && !a.IsDeleted) ||
                              await _dbContext.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted) ||
                              await _dbContext.Clients.AnyAsync(u => u.Email == admin.Email);


            if (nameExists)
                throw new InvalidOperationException("Name already exists in the system.");

            if (emailExists)
                throw new InvalidOperationException("Email already exists in the system.");

            _dbContext.Admins.Add(admin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            // Check if the name or email already exists for another admin or user
            var nameExists = await _dbContext.Admins.AnyAsync(a => a.Id != admin.Id && a.Name == admin.Name && !a.IsDeleted) ||
                             await _dbContext.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted) ||
                             await _dbContext.Clients.AnyAsync(u => u.Name == admin.Name);


            var emailExists = await _dbContext.Admins.AnyAsync(a => a.Id != admin.Id && a.Email == admin.Email && !a.IsDeleted) ||
                              await _dbContext.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted) ||
                              await _dbContext.Clients.AnyAsync(u => u.Email == admin.Email);


            if (nameExists)
                throw new InvalidOperationException("Name already exists in the system.");

            if (emailExists)
                throw new InvalidOperationException("Email already exists in the system.");

            _dbContext.Entry(admin).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }


        public async Task<Admin> GetByAdminEmail(string email)
        {
            return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);
        }


        public async Task<bool> EmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _dbContext.Admins.AnyAsync(u => u.Email == email);
        }

        public async Task DeleteAdminAsync(Admin admin)
        {
            admin.IsDeleted = true;
            _dbContext.Admins.Update(admin);
            await _dbContext.SaveChangesAsync();
        }

        // NEW: Update admin profile (excluding password unless changed)
        public async Task<string> UpdateAdminProfileAsync(int adminId, string name, string phoneNumber, string email)
        {
            var admin = await GetAdminById(adminId);
            if (admin == null) return "Admin not found";

            bool isUpdated = false;

            // Check if Name is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(name) && name != admin.Name)
            {
                bool nameExists = await _dbContext.Admins.AnyAsync(a => a.Id != adminId && a.Name == name && !a.IsDeleted) ||
                                  await _dbContext.Users.AnyAsync(u => u.Name == name && !u.IsDeleted) ||
                                  await _dbContext.Clients.AnyAsync(u => u.Name == name);


                if (nameExists)
                    return "Name already exists in the system.";

                admin.Name = name;
                isUpdated = true;
            }

            // Check if Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(email) && email != admin.Email)
            {
                bool emailExists = await _dbContext.Admins.AnyAsync(a => a.Id != adminId && a.Email == email && !a.IsDeleted) ||
                                   await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted) ||
                                   await _dbContext.Clients.AnyAsync(u => u.Email == email);


                if (emailExists)
                    return "Email already exists in the system.";

                admin.Email = email;
                isUpdated = true;
            }

            // Check if Phone is being updated
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != admin.Phone)
            {
                admin.Phone = phoneNumber;
                isUpdated = true;
            }

            // If no updates were made, return a message indicating no changes
            if (!isUpdated)
                return "No changes have been made.";

            _dbContext.Entry(admin).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return "Profile updated successfully.";
        }



        // NEW: Update admin password
        public async Task<bool> UpdateAdminPasswordAsync(int adminId, string currentPassword, string newPassword)
        {
            var admin = await GetAdminById(adminId);
            if (admin == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, admin.Password))
                return false; // Incorrect current password

            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _dbContext.Entry(admin).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null)
                return null;

            // Generate token using some method (e.g., GUID, JWT token, etc.)
            var token = Guid.NewGuid().ToString();

            // Save token to the database or some temporary storage
            // Optionally, you can expire the token after some time or add more security checks
            admin.ResetToken = token;
            admin.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Expire in 1 hour
            await _dbContext.SaveChangesAsync();

            return token;
        }

        // Reset password for Admin
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email && a.ResetToken == token);
            if (admin == null || admin.ResetTokenExpiry < DateTime.UtcNow)
                return false; // Token is either invalid or expired

            // Hash the new password (assumes you're using a hash function like BCrypt or similar)
            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            admin.ResetToken = null; // Invalidate the token after use
            admin.ResetTokenExpiry = null; // Clear the expiry date
            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}
