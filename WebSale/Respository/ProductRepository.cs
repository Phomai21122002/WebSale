using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebSale.Dto.ProductDetails;
using Azure;
using WebSale.Dto.Products;
using WebSale.Dto.Categories;

namespace WebSale.Respository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dataContext;

        public ProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _dataContext.AddAsync(product);
            await _dataContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await GetProduct(id);
            if (product == null) { return false; }
            _dataContext.Remove(product);
            return await Save();
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _dataContext.Products.Include(p => p.ImageProducts).Include(p => p.Category).Include(p => p.ProductDetail).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ICollection<ProductResultDto>> GetProducts()
        {

            var resProducts = await _dataContext.Products
                .Include(p => p.ImageProducts)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ImageCategories)
                .Include(p => p.ProductDetail)
                .ToListAsync();

            var resultProduct = resProducts.Select(resProduct => new ProductResultDto
            {
                Id = resProduct?.Id ?? 0,
                Name = resProduct?.Name,
                Price = resProduct?.Price ?? 0,
                Urls = resProduct?.ImageProducts?.Select(ip => ip.Url).ToList(),
                Decription = resProduct?.ProductDetail?.Decription,
                DescriptionDetail = resProduct?.ProductDetail?.DescriptionDetail,
                Quantity = resProduct?.ProductDetail?.Quantity ?? 0,
                Tag = resProduct?.ProductDetail?.Tag,
                Sold = resProduct?.ProductDetail?.Sold ?? 0,
                Slug = resProduct?.Slug,
                category = resProduct?.Category == null ? null : new CategoryDto
                {
                    Id = resProduct.Category.Id,
                    Name = resProduct.Category.Name,
                    Description = resProduct.Category.Description,
                    Urls = resProduct?.Category?.ImageCategories?.Select(ic => ic.Url).ToList()
                }
            }).ToList();
            return resultProduct;
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _dataContext.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _dataContext.Update(product);
            return await Save();
        }

        public async Task<bool> SlugExists(string slug)
        {
            return await _dataContext.Products.AnyAsync(p => p.Slug == slug);
        }

        public async Task<ProductResultDto?> GetProductResult(int id)
        {
            var resProduct = await _dataContext.Products
                .Include(p => p.ImageProducts)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ImageCategories)
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == id);
            var resultProduct = new ProductResultDto
            {
                Id = resProduct?.Id ?? 0,
                Name = resProduct?.Name,
                Price = resProduct?.Price ?? 0,
                Urls = resProduct?.ImageProducts?.Select(ip => ip.Url).ToList(),
                Decription = resProduct?.ProductDetail?.Decription,
                DescriptionDetail = resProduct?.ProductDetail?.DescriptionDetail,
                Quantity = resProduct?.ProductDetail?.Quantity ?? 0,
                Tag = resProduct?.ProductDetail?.Tag,
                Sold = resProduct?.ProductDetail?.Sold ?? 0,
                Slug = resProduct?.Slug,
                category = resProduct?.Category == null ? null : new CategoryDto
                {
                    Id = resProduct.Category.Id,
                    Name = resProduct.Category.Name,
                    Description = resProduct.Category.Description,
                    Urls = resProduct?.Category?.ImageCategories?.Select(ic => ic.Url).ToList()
                }
            };

            return resultProduct;
        }
    }
}
