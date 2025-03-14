using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class OrderRepository : IOrderRepository
    {

        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public async Task<Order[]> GetAllOrdersByUserId(int userId)
        {
            return await _dbContext.Orders
                                 .Where(o => o.UserId == userId && !o.IsDeleted)
                                 .ToArrayAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _dbContext.Orders
                                 .Where(o => o.Id == id && !o.IsDeleted)
                                 .Include(o => o.OrderItems)
                                 .FirstOrDefaultAsync();
        }

        public async Task AddOrder(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrder(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
