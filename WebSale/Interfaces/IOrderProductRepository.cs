using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IOrderRepository
    {
        Task<ICollection<Order>> GetOrders(string userId);
        Task<Order> GetOrderByUserId(string userId, int orderId);
        Task<ICollection<Order>> GetOrdersByUserId(string userId);
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Order order);
        Task<bool> Save();
        Task<bool> OrderExists(string userId,int id);
    }
}
