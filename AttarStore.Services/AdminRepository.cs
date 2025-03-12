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

        public async Task<Admin> GetAdminById(int id)
        {
            return await _dbContext.Admins
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task AddAdmin(Admin admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            // Check if the name or email already exists in Admins or Users tables
            var nameExists = await _dbContext.Admins.AnyAsync(a => a.Name == admin.Name && !a.IsDeleted) ||
                             await _dbContext.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted);

            var emailExists = await _dbContext.Admins.AnyAsync(a => a.Email == admin.Email && !a.IsDeleted) ||
                              await _dbContext.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted);

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
                             await _dbContext.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted);

            var emailExists = await _dbContext.Admins.AnyAsync(a => a.Id != admin.Id && a.Email == admin.Email && !a.IsDeleted) ||
                              await _dbContext.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted);

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

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await _dbContext.Admins.FindAsync(id);
            if (admin != null)
            {
                admin.IsDeleted = true; // Soft delete
                await _dbContext.SaveChangesAsync();
            }
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
                                  await _dbContext.Users.AnyAsync(u => u.Name == name && !u.IsDeleted);

                if (nameExists)
                    return "Name already exists in the system.";

                admin.Name = name;
                isUpdated = true;
            }

            // Check if Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(email) && email != admin.Email)
            {
                bool emailExists = await _dbContext.Admins.AnyAsync(a => a.Id != adminId && a.Email == email && !a.IsDeleted) ||
                                   await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);

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
    }
}
