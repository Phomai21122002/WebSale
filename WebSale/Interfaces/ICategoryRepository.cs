using WebSale.Dto.Categories;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<CategoryResultDto>> GetCategories();
        Task<ICollection<TotalSoldProductInCategory>> GetTotalSoldProductInCategory();
        Task<Category?> GetCategory(int id);
        Task<int?> TotalCategory();
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int id);
        Task<bool> Save();
        Task<bool> CategoryExists(int id);
        Task<bool> CategoryNameExists(string name);

    }
}
