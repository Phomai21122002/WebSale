using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class ImageCategoryRepository : ImageRepositoryBase<ImageCategory>, IImageCategoryRepository
    {
        public ImageCategoryRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper, dataContext.ImageCategories) { }
        public async Task<bool> CreateImagesCategory(int idCategory, List<string> imagesCategory)
        {
            return await CreateImages(idCategory, imagesCategory,   
                async id => await _dataContext.Categories.FindAsync(id),
                (url, category) => new ImageCategory { Url =  url, Category = (Category)category });
        }

        public Task<bool> DeleteImagesCategory(int idCategory, List<string> imagesCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<ImageCategory>> GetImageCategoryByIdCategory(int idCategory)
        {
            return await _dataContext.ImageCategories.Where(ic =>ic.Category != null && ic.Category.Id == idCategory).Include(ic => ic.Category).ToListAsync();
        }

        public Task<bool> ImageCategoryExists(int idCategory, string imageCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateImagesCategory(int idCategory, List<string> imagesCategory)
        {
            var imageOfCategory = await GetImageCategoryByIdCategory(idCategory);
            return await UpdateImages(idCategory, imagesCategory, imageOfCategory,
                async id => await _dataContext.Categories.FindAsync(id),    
                (url, category) => new ImageCategory{ Url = url, Category = (Category)category },
                img => img.Url);
        }
    }
}
