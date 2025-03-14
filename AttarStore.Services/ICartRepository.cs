using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByClientIdAsync(int clientId);
        Task AddProductToCart(int clientId, int productId, int quantity);
        Task RemoveProductFromCart(int clientId, int productId);
        Task ClearCart(int clientId);
    }
}
