using AttarStore.Entities;
using AttarStore.Entities.submodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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



        public async Task<string> GetUserRoleByUsername(string username)
        {
            var user = await _dbContext.Users
                .Where(u => !u.IsDeleted)
                .Where(u => u.Role != null)
                .SingleOrDefaultAsync(u => u.Name == username);

            if (user != null)
            {
                return user.Role.ToUpper();
            }

            var admin = await _dbContext.Admins
                .Where(u => !u.IsDeleted)
                .SingleOrDefaultAsync(u => u.Name == username);

            return admin.Role.ToUpper();
        }



        public async Task<IUser[]> GetAllUsers()
        {
            return await _dbContext.Users
                //.Include(U => U.Permissions)
                .Where(u => !u.IsDeleted)
                .ToArrayAsync();
        }


        public async Task<IUser> GetByRefreshToken(string refreshToken)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task AddUser(IUser user)
        {
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users.Where(u => u.Id == id && !u.IsDeleted).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByIdToUpdate(int id)
        {
            return await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(IUser user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var userToDelete = await _dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                _dbContext.Users.Remove(userToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync() > 0);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }


    }
}
