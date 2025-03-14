using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly AppDbContext _dbContext;

        public ManufacturerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Manufacturer[]> GetAllManufacturers()
        {
            return await _dbContext.Manufacturers
                                   .Where(m => !m.IsDeleted)
                                   .Include(c => c.Products)
                                   .ToArrayAsync();
        }

        public async Task<Manufacturer> GetManufacturerByIdAsync(int id)
        {
            return await _dbContext.Manufacturers
                                   .Where(m => m.Id == id && !m.IsDeleted)
                                   .Include(c => c.Products)
                                   .FirstOrDefaultAsync();
        }

        public async Task AddManufacturer(Manufacturer manufacturer)
        {
            await _dbContext.Manufacturers.AddAsync(manufacturer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateManufacturer(Manufacturer manufacturer)
        {
            _dbContext.Manufacturers.Update(manufacturer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteManufacturer(int id)
        {
            var manufacturer = await _dbContext.Manufacturers.FindAsync(id);
            if (manufacturer != null)
            {
                manufacturer.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
