using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _dbContext;

        public InventoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Inventory[]> GetAllInventories()
        {
            return await _dbContext.Inventories
                .Include(i => i.InventoryProducts)
                    .ThenInclude(ip => ip.Product)
                .ToArrayAsync();
        }

        public async Task<Inventory> GetInventoryByIdAsync(int id)
        {
            return await _dbContext.Inventories
                .Include(i => i.InventoryProducts)
                    .ThenInclude(ip => ip.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInventory(Inventory inventory)
        {
            await _dbContext.Inventories.AddAsync(inventory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInventory(Inventory inventory)
        {
            _dbContext.Inventories.Update(inventory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInventory(int id)
        {
            var inventory = await _dbContext.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _dbContext.Inventories.Remove(inventory);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
