using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebSale.Dto.Orders;
using WebSale.Dto.Addresses;
using WebSale.Dto.Products;
using WebSale.Dto.Categories;
using WebSale.Dto.Users;

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

        public async Task<OrderResultDetailDto?> GetOrderResultByUserId(string userId, int orderId)
        {
            var order = await _dataContext.Orders
                .Where(o => o.Id == orderId && o.User != null && o.User.Id == userId)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ImageProducts)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Category)
                            .ThenInclude(c => c.ImageCategories)
                .FirstOrDefaultAsync();

            if (order == null) return null;

            return new OrderResultDetailDto
            {
                Id = order.Id,
                Name = order.Name,
                Status = order.Status,
                Total = order.Total,
                CountProduct = order.OrderProducts
                    .Where(op => op.Status == order.Status)
                    .Sum(op => op.Quantity),
                CreateOrder = order.CreatedAt,
                User = new UserResultDto
                {
                    Id = order.User.Id,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Email = order.User.Email,
                    Phone = order.User.Phone,
                    url = order.User.url,
                },
                Products = order.OrderProducts.Where(op => op.Status == order.Status).Select(op => new ProductOrderResultDto
                {
                    Id = op.Product.Id,
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    Quantity = op.Quantity,
                    Slug = op.Product.Slug,
                    Description = op.Product.ProductDetail?.Description,
                    Sold = op.Product.ProductDetail?.Sold ?? 0,
                    Tag = op.Product.ProductDetail?.Tag,
                    ExpiryDate = op.Product.ProductDetail?.ExpiryDate,
                    Status = op.Status,
                    Urls = op.Product.ImageProducts != null
                        ? op.Product.ImageProducts.Select(ip => ip.Url).ToList()
                        : new List<string>(),
                    category = op.Product.Category != null
                        ? new CategoryDto
                        {
                            Id = op.Product.Category.Id,
                            Name = op.Product.Category.Name,
                            Description = op.Product.Category.Description,
                            Urls = op.Product.Category.ImageCategories != null
                                ? op.Product.Category.ImageCategories.Select(ic => ic.Url).ToList()
                                : new List<string>()
                        }
                        : null
                }).ToList()
            };
        }


        public async Task<ICollection<Order>> GetOrdersByUserId(string userId)
        {
            var order = await _dataContext.Orders.Where(c => c.User != null && c.User.Id == userId).ToListAsync();
            return order;
        }
        public async Task<ICollection<OrderResultDto>> GetOrdersResultByUserId(string userId, int status)
        {
            var orders = await _dataContext.Orders
                .Where(o => o.User != null && o.User.Id == userId && o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ImageProducts)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Category)
                            .ThenInclude(c => c.ImageCategories)
                .ToListAsync();

            return orders.Select(order => new OrderResultDto {
                Id = order.Id,
                Name = order.Name,
                Status = order.Status,
                Total = order.Total,
                CountProduct = order.OrderProducts.Sum(op => op.Quantity),
                CreateOrder = order.CreatedAt,
                User = order.User != null ? new UserBaseDto
                {
                    Email = order.User.Email,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Phone = order.User.Phone,
                    url = order.User.url
                } : null,
            }).ToList();
        }

        public async Task<Order?> GetOrderByUserId(string userId, int orderId)
        {
            return await _dataContext.Orders
                .Where(o => o.Id == orderId && o.User != null && o.User.Id == userId)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ProductOfOrderExists(string userId, int orderId, int productId)
        {
            return await _dataContext.Orders
                .Where(o => o.Id == orderId && o.User != null && o.User.Id == userId)
                .SelectMany(o => o.OrderProducts)
                .AnyAsync(op => op.ProductId == productId);
        }
    }
}
