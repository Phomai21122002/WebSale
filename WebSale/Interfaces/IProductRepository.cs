using WebSale.Dto.Products;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IProductRepository
    {
        Task<PageResult<ProductResultDto>> GetProducts(QueryFindSoftPaginationDto queryProducts);
        Task<PageResult<ProductResultDto>> GetProductsByIdCategory(int categoryId, QuerySoftPaginationDto queryProducts);
        Task<Product?> GetProduct(int id);
        Task<int?> TotalProduct();
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
