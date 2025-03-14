using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly AppDbContext _dbContext;

        public BillingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Billing[]> GetAllBillingsByOrderId(int orderId)
        {
            return await _dbContext.Billings
                .Where(b => b.OrderId == orderId)
                .ToArrayAsync();
        }

        public async Task<Billing> GetBillingByIdAsync(int id)
        {
            return await _dbContext.Billings
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddBilling(Billing billing)
        {
            await _dbContext.Billings.AddAsync(billing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateBilling(Billing billing)
        {
            _dbContext.Billings.Update(billing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteBilling(int id)
        {
            var billing = await GetBillingByIdAsync(id);
            if (billing != null)
            {
                _dbContext.Billings.Remove(billing);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
