using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IImageProductRepository
    {
        Task<ICollection<ImageProduct>> GetImageProductByIdProduct(int idProduct);
        Task<bool> CreateImageProduct(int idProduct, List<string> imagesProduct);
        Task<bool> UpdateImageProduct(int idProduct, List<string> imagesProduct);
        Task<bool> DeleteImageProduct(ICollection<ImageProduct> imagesProduct);
        Task<bool> Save();
        Task<bool> ImageProductExists(int idProduct, string imageProduct);
    }
}
