using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface IInventoryRepository
    {
        Task<Inventory[]> GetAllInventories();
        Task<Inventory> GetInventoryByIdAsync(int id);
        Task AddInventory(Inventory inventory);
        Task UpdateInventory(Inventory inventory);
        Task DeleteInventory(int id);
    }
}
