using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int id);
        Task<bool> Save();
        Task<bool> CategoryExists(int id);
        Task<bool> CategoryNameExists(string name);

    }
}
