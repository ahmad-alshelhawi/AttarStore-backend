using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface IProductRepository
    {
        Task<Product[]> GetAllProducts();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int id);
    }
}
