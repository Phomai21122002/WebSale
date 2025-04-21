using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IImageCategoryRepository 
    {
        Task<ICollection<ImageCategory>> GetImageCategoryByIdCategory(int idCategory);
        Task<bool> CreateImagesCategory(int idCategory, List<string> imagesCategory);
        Task<bool> UpdateImagesCategory(int idCategory, List<string> imagesCategory);
        Task<bool> DeleteImagesCategory(int idCategory, List<string> imagesCategory);
        Task<bool> ImageCategoryExists(int idCategory, string imageCategory);
    }
}
