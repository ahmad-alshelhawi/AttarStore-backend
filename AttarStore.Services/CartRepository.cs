using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;

        public CartRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<Cart> GetCartByClientIdAsync(int clientId)
        {
            return await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == clientId);
        }

        public async Task AddProductToCart(int clientId, int productId, int quantity)
        {
            var cart = await GetCartByClientIdAsync(clientId);
            if (cart == null)
            {
                cart = new Cart { UserId = clientId, CartItems = new List<CartItem>() };
                await _dbContext.Carts.AddAsync(cart);
            }

            var cartItem = new CartItem { ProductId = productId, Quantity = quantity };
            cart.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveProductFromCart(int clientId, int productId)
        {
            var cart = await GetCartByClientIdAsync(clientId);
            var item = cart?.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                cart.CartItems.Remove(item);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearCart(int clientId)
        {
            var cart = await GetCartByClientIdAsync(clientId);
            if (cart != null)
            {
                cart.CartItems.Clear();
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
