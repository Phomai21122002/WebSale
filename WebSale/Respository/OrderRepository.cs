using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WebSale.Respository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _dataContext;

        public OrderRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _dataContext.AddAsync(order);
            await _dataContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrder(Order order)
        {
            _dataContext.Remove(order);
            return await Save();
        }

        public async Task<ICollection<Order>> GetOrders(string userId)
        {
            return await _dataContext.Orders.Where(o => o.User != null && o.User.Id == userId).ToListAsync();
        }

        public async Task<bool> OrderExists(string userId, int id)
        {
            return await _dataContext.Orders.AnyAsync(o => o.Id == id && o.User != null && o.User.Id == userId);
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            _dataContext.Update(order);
            await _dataContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByUserId(string userId, int orderId)
        {
            return await _dataContext.Orders
                .Where(o => o.Id == orderId && o.User != null && o.User.Id == userId)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .FirstOrDefaultAsync(); ;
        }

        public async Task<ICollection<Order>> GetOrdersByUserId(string userId)
        {
            var order = await _dataContext.Orders.Where(c => c.User != null && c.User.Id == userId).ToListAsync();
            return order;
        }
    }
}
