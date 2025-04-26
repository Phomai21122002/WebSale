using WebSale.Dto.Products;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IProductRepository
    {
        Task<ICollection<ProductResultDto>> GetProducts();
        Task<Product?> GetProduct(int id);
        Task<int?> GetIdProductBySlug(string slug);
        Task<ProductResultDto?> GetProductResult(int id);
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
        Task<bool> Save();
        Task<bool> ProductExists(int id);
        Task<bool> SlugExists(string slug);
                    
    }
}
