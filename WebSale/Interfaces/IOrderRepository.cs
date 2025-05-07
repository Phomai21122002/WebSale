using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IOrderProductRepository
    {
        Task<OrderProduct> GetOrderProduct(string userId, int orderId, int productId);
        Task<ICollection<OrderProduct>> GetOrderProducts(string userId, int orderId);
        Task<ICollection<OrderProduct>> CreateOrderProduct(ICollection<OrderProduct> orderProduct);
        Task<OrderProduct> UpdateOrderProduct(OrderProduct orderProduct);
        Task<ICollection<OrderProduct>> UpdateOrderProducts(ICollection<OrderProduct> orderProducts);
        Task<bool> DeleteOrderProduct(OrderProduct orderProduct);
        Task<bool> Save();
        Task<bool> OrderProductExists(int id);
    }
}
