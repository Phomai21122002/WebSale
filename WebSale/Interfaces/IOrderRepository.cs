using WebSale.Dto.Orders;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Models;

namespace WebSale.Interfaces
{
    

    public interface IOrderRepository
    {
        Task<ICollection<Order>> GetOrders(string userId);
        Task<Order> GetOrderByUserId(string userId, int orderId);
        Task<Order> GetOrderById(int orderId);
        Task<int?> TotalOrder();
        Task<OrderResultStatisticDto> GetOrderStatistic();
        Task<OrderResultDetailDto> GetOrderResultByUserId(string userId, int orderId);
        Task<PageResult<OrderResultDto>> GetOrdersResultByUserId(string userId, int status, QueryFindPaginationDto queryOrders);
        Task<PageResult<OrderResultDto>> GetOrdersResultByAdmin(int status, QueryFindSoftPaginationDto queryOrders);
        Task<ICollection<Order>> GetOrdersByUserId(string userId);
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Order order);
        Task<bool> Save();
        Task<bool> OrderExists(string userId, int id);
        Task<bool> ProductOfOrderExists(string userId, int orderId, int productId);
    }
}
