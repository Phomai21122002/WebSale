using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IOrderProductRepository
    {
        Task<ICollection<OrderProduct>> CreateOrderProduct(ICollection<OrderProduct> orderProduct);
        Task<OrderProduct> UpdateOrderProduct(OrderProduct orderProduct);
        Task<bool> DeleteOrderProduct(OrderProduct orderProduct);
        Task<bool> Save();
        Task<bool> OrderProductExists(int id);
    }
}
