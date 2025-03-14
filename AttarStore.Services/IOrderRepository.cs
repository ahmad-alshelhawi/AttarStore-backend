using AttarStore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface IOrderRepository
    {
        Task<Order[]> GetAllOrdersByUserId(int userId);
        Task<Order> GetOrderByIdAsync(int id);
        Task AddOrder(Order order);
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
    }
}
