using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSale.Dto.Categories;

namespace WebSale.Respository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;

        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CategoryNameExists(string name)
        {
            return await _dataContext.Categories.AnyAsync(c => c.Name == name);

        }

        public async Task<bool> CategoryExists(int id)
        {
            return await _dataContext.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategory(Category category)
        {
            await _dataContext.AddAsync(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await GetCategory(id);
            if (category == null)
            {
                return false;
            }

            _dataContext.Remove(category);
            return await Save();
        }

        public async Task<ICollection<CategoryResultDto>> GetCategories()
        {
            return await _dataContext.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryResultDto {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CountProduct = c.Products!.Count(),
                    ImageCategories = c.ImageCategories,
                    IsDelete = c.IsDeleted
                }).ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            return await _dataContext.Categories.Include(c => c.ImageCategories).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
           _dataContext.Update(category);
           await  _dataContext.SaveChangesAsync();
           return category;
        }
        public async Task<int?> TotalCategory()
        {
            int count = await _dataContext.Categories.Where(c => !c.IsDeleted).CountAsync();
            return count;
        }
    }
}
