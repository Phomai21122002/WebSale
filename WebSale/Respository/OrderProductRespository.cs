using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class OrderProductRespository : IOrderProductRepository
    {
        private readonly DataContext _dataContext;

        public OrderProductRespository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ICollection<OrderProduct>> CreateOrderProduct(ICollection<OrderProduct> orderProduct)
        {
            await _dataContext.AddRangeAsync(orderProduct);
            await _dataContext.SaveChangesAsync();
            return orderProduct;
        }

        public Task<bool> DeleteOrderProduct(OrderProduct orderProduct)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderProduct?> GetOrderProduct(string userId, int orderId, int productId)
        {
            return await _dataContext.OrderProducts
                .Include(op => op.Order)
                    .ThenInclude(o => o.User)
                .Include(op => op.Product)
                .FirstOrDefaultAsync(op =>
                    op.OrderId == orderId &&
                    op.ProductId == productId &&
                    op.Order.User != null &&
                    op.Order.User.Id == userId);
        }

        public async Task<ICollection<OrderProduct>> GetOrderProducts(string userId, int orderId)
        {
            return await _dataContext.OrderProducts.Where(op => op.Order.User != null && op.Order.User.Id == userId && op.OrderId == orderId).ToListAsync();
        }

        public Task<bool> OrderProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public async Task<OrderProduct> UpdateOrderProduct(OrderProduct orderProduct)
        {
            _dataContext.Update(orderProduct);
            await _dataContext.SaveChangesAsync();
            return orderProduct;
        }

        public async Task<ICollection<OrderProduct>> UpdateOrderProducts(ICollection<OrderProduct> orderProducts)
        {
            _dataContext.UpdateRange(orderProducts);
            await _dataContext.SaveChangesAsync();
            return orderProducts;
        }
    }
}
