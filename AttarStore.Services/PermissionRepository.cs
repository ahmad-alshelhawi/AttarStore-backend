
using AttarStore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _dbContext;

        public PermissionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> checkPermission(string permission, string userId)
        {
            var firstPermission = await _dbContext.PermissionUser
                .Where(u => u.Permission.Name == permission && u.User.Name == userId).FirstOrDefaultAsync();
            if (firstPermission != null)
            {
                return true;
            }
            return false;
        }



        public async Task<PermissionUser[]> GetPermissionsById(string name)
        {
            Console.WriteLine("jj");
            Console.WriteLine(name);

            return await _dbContext.PermissionUser
                .Include(u => u.Permission)
                .Include(u => u.User)
                .Where(u => u.User.Name == name)
                .ToArrayAsync();
        }
    }
}
