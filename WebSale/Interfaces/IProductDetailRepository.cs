using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IProductDetailRepository
    {
        Task<ICollection<ProductDetail>> GetProductDetails(ICollection<int> productDetailsId);
        Task<ProductDetail?> GetProductDetail(int id);
        Task<ProductDetail> CreateProductDetail(ProductDetail productDetail);
        Task<bool> UpdateProductDetail(ProductDetail productDetail);
        Task<bool> UpdateProductDetails(ICollection<ProductDetail> productDetails);
        Task<bool> DeleteProductDetail(ProductDetail productDetail);
        Task<bool> Save();
        Task<bool> ProductDetailExists(int id);
    }
}
