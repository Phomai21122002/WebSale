using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebSale.Respository
{
    public class ImageProductRepository : ImageRepositoryBase<ImageProduct>, IImageProductRepository
    {
        public ImageProductRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper, dataContext.ImageProducts) {}

        public async Task<bool> CreateImageProduct(int idProduct, List<string> imagesProduct)
        {
            return await base.CreateImages(idProduct, imagesProduct,
                async id => await _dataContext.Products.FindAsync(id),
                (url, product) => new ImageProduct { Url = url, Product = (Product)product });
        }

        public async Task<bool> DeleteImageProduct(ICollection<ImageProduct> imagesProduct)
        {
            if(imagesProduct.Count <= 0)
            {
                return true;
            }
            foreach (var image in imagesProduct)
            {
                _dataContext.ImageProducts.Remove(image);
            }
            return await Save();
        }

        public async Task<ICollection<ImageProduct>> GetImageProductByIdProduct(int idProduct)
        {
            return await _dataContext.ImageProducts.Where(ip => ip.Product.Id == idProduct).Include(ip => ip.Product).ToListAsync();
        }

        public Task<bool> ImageProductExists(int idProduct, string imageProduct)
        {
            throw new NotImplementedException();
        }

        public new async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateImageProduct(int idProduct, List<string> imagesProduct)
        {
            var imagesOfProduct = await GetImageProductByIdProduct(idProduct);
            return await UpdateImages(idProduct, imagesProduct, imagesOfProduct,
                async id => await _dataContext.Products.FindAsync(id),
                (url, product) => new ImageProduct { Url = url, Product =(Product)product},
                img => img.Url);
        }
    }
}
