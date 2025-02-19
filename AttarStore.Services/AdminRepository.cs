using AttarStore.Entities;
using AttarStore.Entities.submodels;
using Microsoft.EntityFrameworkCore;

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
                //.Include(U => U.Permissions)
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
            _dbContext.Admins.Add(admin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _dbContext.Entry(admin).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Admins.AnyAsync(u => u.Email == email);
        }

    }
}
