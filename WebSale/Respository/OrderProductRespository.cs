using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Dto.Addresses;
using WebSale.Dto.Categories;
using WebSale.Dto.Orders;
using WebSale.Dto.Products;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
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

        public async Task<bool> DeleteOrderProducts(ICollection<OrderProduct> orderProducts)
        {
            _dataContext.RemoveRange(orderProducts);
            return await Save();
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
            return await _dataContext.OrderProducts
                .Where(op => op.Order.User != null && op.Order.User.Id == userId && op.OrderId == orderId)
                .Include(op => op.Product)
                    .ThenInclude(p => p.ProductDetail)
                .ToListAsync();
        }

        public async Task<PageResult<OrderProductCancelDto>> GetProductOrdersResultCanceled(string userId, int status, QueryPaginationDto queryPaginationDto)
        {
            var query = _dataContext.OrderProducts
                .Where(op =>
                    op.Order.User != null &&
                    op.Order.User.Id == userId &&
                    (status == 0 || op.Status == status)
                )
                .Include(op => op.Order)
                    .ThenInclude(o => o.User)
                .Include(op => op.Product)
                    .ThenInclude(p => p.ProductDetail)
                .Include(op => op.Product)
                    .ThenInclude(p => p.ImageProducts)
                .Include(op => op.Product)
                    .ThenInclude(p => p.Category)
                        .ThenInclude(c => c.ImageCategories)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            if (queryPaginationDto.PageNumber > 0 && queryPaginationDto.PageSize > 0)
            {
                query = query
                    .Skip((queryPaginationDto.PageNumber - 1) * queryPaginationDto.PageSize)
                    .Take(queryPaginationDto.PageSize);
            }

            var canceledProducts = await query.ToListAsync();

            var result = canceledProducts.Select(op => new OrderProductCancelDto
            {
                Id = op.Order.Id,
                Name = op.Order.Name,
                Status = op.Order.Status,
                Total = op.Order.Total,
                CreateOrder = op.Order.CreatedAt,
                User = new UserResultDto
                {
                    Id = op.Order.User.Id,
                    FirstName = op.Order.User.FirstName,
                    LastName = op.Order.User.LastName,
                    Email = op.Order.User.Email,
                    Phone = op.Order.User.Phone,
                    url = op.Order.User.url
                },
                Product = new ProductOrderResultDto
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
                    Urls = op.Product.ImageProducts?.Select(ip => ip.Url).ToList() ?? new List<string>(),
                    category = op.Product.Category != null
                        ? new CategoryDto
                        {
                            Id = op.Product.Category.Id,
                            Name = op.Product.Category.Name,
                            Description = op.Product.Category.Description,
                            Urls = op.Product.Category.ImageCategories?.Select(ic => ic.Url).ToList() ?? new List<string>()
                        }
                        : null
                }
            }).ToList();

            return new PageResult<OrderProductCancelDto>
            {
                TotalCount = totalCount,
                PageNumber = queryPaginationDto.PageNumber,
                PageSize = queryPaginationDto.PageSize,
                Datas = result
            };
        }

        public Task<bool> OrderProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
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
