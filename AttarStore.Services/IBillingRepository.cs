using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface IBillingRepository
    {
        Task<Billing[]> GetAllBillingsByOrderId(int orderId);
        Task<Billing> GetBillingByIdAsync(int id);
        Task AddBilling(Billing billing);
        Task UpdateBilling(Billing billing);
        Task DeleteBilling(int id);
    }
}
