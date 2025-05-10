using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebSale.Dto.Carts;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Dto.Orders;

namespace WebSale.Respository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _dataContext;

        public CartRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CartExists(int id)
        {
            return await _dataContext.Carts.AnyAsync(c => c.Id == id);
        }

        public async Task<Cart> CreateCart(Cart cart)
        {
            await _dataContext.AddAsync(cart);
            await _dataContext.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteCart(Cart cart)
        {
            _dataContext.Remove(cart);
            return await Save();
        }

        public async Task<Cart?> GetCart(string userId, int cartId)
        {
            return await _dataContext.Carts.Where(c => c.Id == cartId && c.User != null && c.User.Id == userId).Include(c => c.User).Include(c => c.Product).FirstOrDefaultAsync();
        }

        public async Task<Cart?> GetCartByIdProduct(string userId, int productId)
        {
            return await _dataContext.Carts.Where(c => c.Product != null && c.Product.Id == productId && c.User != null && c.User.Id == userId).Include(c => c.User).Include(c => c.Product).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Cart>> GetCartsByCartsId(string userId, CreateOrderDto createOrderDtos)
        {
            var cartRes = await _dataContext.Carts
                .Where(c => createOrderDtos.CartsId.Contains(c.Id) && c.User != null && c.User.Id == userId)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ProductDetail)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ImageProducts)
                .ToListAsync();
            return cartRes;
        }

        public async Task<CartDetailDto> GetCartByUserId(string userId, int cartId)
        {
            var cartRes = await _dataContext.Carts
                .Where(c => c.Id == cartId && c.User != null && c.User.Id == userId)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.Category)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ProductDetail)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ImageProducts)
                .FirstOrDefaultAsync();

            if (cartRes == null)
                return new CartDetailDto();

            var resultCart = new CartDetailDto
            {
                Id = cartRes.Id,
                Total = cartRes.Quantity * cartRes.Product?.Price ?? 0,
                IsSelectedForOrder = cartRes.IsSelectedForOrder,
                product = new ProductDetailResultDto
                {
                    Id = cartRes.Product?.Id ?? 0,
                    Name = cartRes.Product?.Name,
                    Price = cartRes.Product?.Price ?? 0,
                    Urls = cartRes.Product?.ImageProducts?.Select(ip => ip.Url).ToList(),
                    Description = cartRes.Product?.ProductDetail?.Description,
                    DescriptionDetail = cartRes.Product?.ProductDetail?.GetDescriptionFromFile(),
                    Quantity = cartRes.Quantity,
                    Tag = cartRes.Product?.ProductDetail?.Tag,
                    Sold = cartRes.Product?.ProductDetail?.Sold ?? 0,
                    Slug = cartRes.Product.Slug
                },
                category = cartRes.Product?.Category == null ? null : new CategoryDto
                {
                    Id = cartRes.Product.Category.Id,
                    Name = cartRes.Product.Category.Name,
                    Description = cartRes.Product.Category.Description,
                }
            };

            return resultCart;
        }


        public async Task<ICollection<Cart>> GetCarts(string userId)
        {
            return await _dataContext.Carts.Where(c => c.User != null && c.User.Id == userId)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ProductDetail)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ImageProducts).ToListAsync();
        }

        public async Task<ICollection<CartDetailDto>> GetCartsByUserId(string userId)
        {
            var cartRes = await _dataContext.Carts
                .Where(c => c.User != null && c.User.Id == userId)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.Category)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ProductDetail)
                .Include(c => c.Product)
                    .ThenInclude(cp => cp.ImageProducts)
                .ToListAsync();

            if (cartRes == null)
                return new List<CartDetailDto>();

            var resultCart = cartRes.Select(cp => new CartDetailDto
            {
                Id = cp.Id,
                Total = cp.Quantity * cp.Product?.Price ?? 0,
                IsSelectedForOrder = cp.IsSelectedForOrder,
                product = new ProductDetailResultDto
                {
                    Id = cp.Product?.Id ?? 0,
                    Name = cp.Product?.Name,
                    Price = cp.Product?.Price ?? 0,
                    Urls = cp.Product?.ImageProducts?.Select(ip => ip.Url).ToList(),
                    Description = cp.Product?.ProductDetail?.Description,
                    DescriptionDetail = cp.Product?.ProductDetail?.GetDescriptionFromFile(),
                    Quantity = cp.Quantity,
                    Tag = cp.Product?.ProductDetail?.Tag,
                    Sold = cp.Product?.ProductDetail?.Sold ?? 0,
                    Slug = cp.Product.Slug
                },
                category = cp.Product?.Category == null ? null : new CategoryDto
                {
                    Id = cp.Product.Category.Id,
                    Name = cp.Product.Category.Name,
                    Description = cp.Product.Category.Description,
                }
            }).ToList();

            return resultCart;
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCart(Cart cart)
        {
            _dataContext.Carts.Attach(cart);
            _dataContext.Entry(cart).Property(c => c.Quantity).IsModified = true;

            _dataContext.Entry(cart).Property(c => c.Id).IsModified = false;
            _dataContext.Entry(cart).Reference(c => c.User).IsModified = false;
            _dataContext.Entry(cart).Reference(c => c.Product).IsModified = false;
            return await Save();
        }
        public async Task<bool> UpdateCarts(IEnumerable<Cart> carts)
        {
            foreach (var cart in carts)
            {
                _dataContext.Carts.Attach(cart);
                _dataContext.Entry(cart).Property(c => c.IsSelectedForOrder).IsModified = true;
                _dataContext.Entry(cart).Property(c => c.UpdatedAt).IsModified = true;
            }

            return await Save();
        }
        public async Task<bool> DeleteCarts(ICollection<Cart> carts)
        {
            _dataContext.Carts.RemoveRange(carts);
            return await Save();
        }
    }
}
