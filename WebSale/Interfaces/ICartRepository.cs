using WebSale.Dto.Carts;
using WebSale.Dto.Orders;
using WebSale.Dto.ProductDetails;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface ICartRepository
    {
        Task<ICollection<Cart>> GetCarts(string userId);
        Task<Cart?> GetCart(string userId, int cartId);
        Task<CartDetailDto> GetCartByUserId(string userId, int cartId);
        Task<ICollection<CartDetailDto>> GetCartsByUserId(string userId);
        Task<ICollection<Cart>> GetCartsByCartsId(string userId, CreateOrderDto createOrderDtos);
        Task<Cart> CreateCart(Cart cart);
        Task<bool> UpdateCart(Cart cart);
        Task<bool> DeleteCart(Cart cart);
        Task<bool> DeleteCarts(ICollection<Cart> carts);
        Task<bool> Save();
        Task<bool> CartExists(int id);
    }
}
