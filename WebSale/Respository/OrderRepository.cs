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
using MailKit.Search;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSale.Extensions;
using WebSale.Dto.QueryDto;
using FPS_ReviewAPI.Dto;

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
                    .ThenInclude(op => op.UserAddresses)
                        .ThenInclude(op => op.Address)
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
                PaymentMethod = order.PaymentMethod,
                IsPayment = order.IsPayment,
                CreateOrder = order.CreatedAt,
                User = new UserResultDto
                {
                    Id = order.User.Id,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Email = order.User.Email,
                    Phone = order.User.Phone,
                    url = order.User.url,
                    Addresses = order.User.UserAddresses!.Select(ud => new AddressDto
                    {
                        Id = ud!.Address!.Id,
                        Name = ud!.Address.Name,
                        Code = ud!.Address.Code,
                        IsDefault = ud!.IsDefault
                    }).ToList()
                },
                Products = order.OrderProducts.Where(op => op.Status == order.Status).Select(op => new ProductOrderResultDto
                {
                    Id = op.Product.Id,
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    Quantity = op.Product.ProductDetail?.Quantity,
                    Count = op.Quantity,
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
            return await _dataContext.Orders.Where(c => c.User != null && c.User.Id == userId).ToListAsync();
        }
        public async Task<PageResult<OrderResultDto>> GetOrdersResultByUserId(string userId, int status, QueryFindPaginationDto queryOrders)
        {
            var query = _dataContext.Orders
                .Where(o => o.User != null && o.User.Id == userId && o.Status == status)
                .Include(o => o.User)
                    .ThenInclude(ua => ua.UserAddresses)
                        .ThenInclude(a => a.Address)
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
                .OrderByDescending(o => o.CreatedAt);

            var orders = await query.ToListAsync();


            if (!string.IsNullOrEmpty(queryOrders.Name))
            {
                var keyword = RemoveDiacritics.RemoveDiacriticsChar(queryOrders.Name.ToLower());
                orders = orders.Where(p =>
                    !string.IsNullOrEmpty(p.Name) &&
                     RemoveDiacritics.RemoveDiacriticsChar(p.Name.ToLower()).Contains(keyword)).ToList();
            }

            var totalCount = orders.Count;

            if (queryOrders.PageNumber > 0 && queryOrders.PageSize > 0)
            {
                orders = orders
                    .Skip((queryOrders.PageNumber - 1) * queryOrders.PageSize)
                    .Take(queryOrders.PageSize)
                    .ToList();
            }

            var resultOrders = orders.Select(order => new OrderResultDto
            {
                Id = order.Id,
                Name = order.Name,
                Status = order.Status,
                Total = order.Total,
                CountProduct = order.OrderProducts
                    .Where(op => op.Status == order.Status)
                    .Sum(op => op.Quantity),
                CreateOrder = order.CreatedAt,
                User = order.User != null ? new UserResultDto
                {
                    Id = order.User.Id,
                    Email = order.User.Email,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Phone = order.User.Phone,
                    url = order.User.url,
                    Addresses = order.User.UserAddresses!
                    .Select(ud => new AddressDto
                    {
                        Id = ud.Address!.Id,
                        Name = ud.Address!.Name!,
                        Code = ud.Address.Code,
                        IsDefault = ud.IsDefault
                    })
                    .ToList(),
                    Role = order.User.Role
                } : null,

            }).ToList();

            return new PageResult<OrderResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryOrders.PageNumber,
                PageSize = queryOrders.PageSize,
                Datas = resultOrders
            };
        }

        public async Task<PageResult<OrderResultDto>> GetOrdersResultByAdmin(int status, QueryFindSoftPaginationDto queryOrders)
        {
            var query = _dataContext.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                    .ThenInclude(ua => ua.UserAddresses)
                        .ThenInclude(a => a.Address)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ImageProducts)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Category)
                            .ThenInclude(c => c.ImageCategories);

            var orders = await query.ToListAsync();

            if (!string.IsNullOrEmpty(queryOrders.Name))
            {
                var keyword = RemoveDiacritics.RemoveDiacriticsChar(queryOrders.Name.ToLower());
                orders = orders.Where(p =>
                    !string.IsNullOrEmpty(p.Name) &&
                     RemoveDiacritics.RemoveDiacriticsChar(p.Name.ToLower()).Contains(keyword)).ToList();
            }

            var totalCount = orders.Count;

            // Sắp xếp
            if (!string.IsNullOrEmpty(queryOrders.SortBy))
            {
                orders = queryOrders.SortBy.ToLower() switch
                {
                    "date" => queryOrders.isDecsending
                        ? orders.OrderByDescending(p => p.CreatedAt).ToList()
                        : orders.OrderBy(p => p.CreatedAt).ToList(),

                    "name" => queryOrders.isDecsending
                        ? orders.OrderByDescending(p => p.Name).ToList()
                        : orders.OrderBy(p => p.Name).ToList(),

                    _ => orders // nếu không khớp sortBy thì không sắp xếp
                };
            }

            if (queryOrders.PageNumber > 0 && queryOrders.PageSize > 0)
            {
                orders = orders
                    .Skip((queryOrders.PageNumber - 1) * queryOrders.PageSize)
                    .Take(queryOrders.PageSize)
                    .ToList();
            }

            var resultOrders = orders.Select(order => new OrderResultDto
            {
                Id = order.Id,
                Name = order.Name,
                Status = order.Status,
                Total = order.Total,
                CountProduct = order.OrderProducts
                    .Where(op => op.Status == order.Status)
                    .Sum(op => op.Quantity),
                CreateOrder = order.CreatedAt,
                User = order.User != null ? new UserResultDto
                {
                    Id = order.User.Id,
                    Email = order.User.Email,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Phone = order.User.Phone,
                    url = order.User.url,
                    Addresses = order.User.UserAddresses!
                    .Select(ud => new AddressDto
                    {
                        Id = ud.Address!.Id,
                        Name = ud.Address!.Name!,
                        Code = ud.Address.Code,
                        IsDefault = ud.IsDefault
                    })
                    .ToList(),
                    Role = order.User.Role
                } : null,
            }).ToList();

            return new PageResult<OrderResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryOrders.PageNumber,
                PageSize = queryOrders.PageSize,
                Datas = resultOrders
            };
        }

        public async Task<Order?> GetOrderByUserId(string userId, int orderId)
        {
            return await _dataContext.Orders
                .Where(o => o.Id == orderId && o.User != null && o.User.Id == userId)
                .Include(o => o.User)
                    .ThenInclude(u => u.UserAddresses
                        .Where(ua => ua.IsDefault && ua.UserId == userId))
                        .ThenInclude(ua => ua.Address)
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

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _dataContext.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.ProductDetail)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderResultStatisticDto> GetOrderStatistic()
        {
            var orders = await _dataContext.Orders
                .Include(o => o.User)
                    .ThenInclude(ua => ua.UserAddresses)
                        .ThenInclude(a => a.Address)
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
            if (orders == null || !orders.Any())
            {
                return new OrderResultStatisticDto
                {
                    AmountPending = 0,
                    AmountShipped = 0,
                    AmountRecieved = 0,
                    AmountCancelled = 0,
                };
            }

            var resultOrderStatistic = new OrderResultStatisticDto
            {
                AmountPending = orders.Count(o => o.Status == (int)OrderStatus.Pending),
                AmountShipped = orders.Count(o => o.Status == (int)OrderStatus.Processing),
                AmountRecieved = orders.Count(o => o.Status == (int)OrderStatus.Completed),
                AmountCancelled = orders.Count(o => o.Status == (int)OrderStatus.Cancelled),
            };

            return resultOrderStatistic;
        }

        public async Task<int?> TotalOrder()
        {
            int count = await _dataContext.Orders.CountAsync();
            return count;
        }
    }
}
