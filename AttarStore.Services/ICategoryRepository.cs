using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category[]> GetAllCategories();
        Task<Category> GetByCategoryIdAsync(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int id);
    }
}
