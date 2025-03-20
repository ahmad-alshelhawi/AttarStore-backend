using AttarStore.Entities;
using AttarStore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Category[]> GetAllCategories()
        {
            return await _dbContext.Categories
                                   .Include(c => c.Products)
                                   .Include(c => c.Categories)
                                   .ToArrayAsync();
        }

        public async Task<Category?> GetByCategoryIdAsync(int id)
        {
            return await _dbContext.Categories
                                   .Include(c => c.Products)
                                   .Include(c => c.Categories)
                                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            var existingCategory = await _dbContext.Categories.FindAsync(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.ParentId = category.ParentId;

                // Only update the ImageUrl if a new image is uploaded
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    existingCategory.ImageUrl = category.ImageUrl;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Category[]> GetByParentIdAsync(int? parentId)
        {
            return await _dbContext.Categories
                                   .Where(c => c.ParentId == parentId)
                                   .ToArrayAsync();
        }
    }
}
