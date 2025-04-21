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

        public Task<bool> OrderProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<OrderProduct> UpdateOrderProduct(OrderProduct orderProduct)
        {
            throw new NotImplementedException();
        }
    }
}
