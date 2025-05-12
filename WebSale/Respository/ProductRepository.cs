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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using WebSale.Extensions;

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

        public async Task<PageResult<ProductResultDto>> GetProducts(QueryProducts queryProducts)
        {
            var query = _dataContext.Products
                .Include(p => p.ImageProducts)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ImageCategories)
                .Include(p => p.ProductDetail)
                .AsQueryable();

            // Lọc theo tên trước
            if (!string.IsNullOrEmpty(queryProducts.Name))
            {
                var keyword = RemoveDiacritics.RemoveDiacriticsChar(queryProducts.Name.ToLower());
                query = query.Where(p =>
                    !string.IsNullOrEmpty(p.Name) &&
                    RemoveDiacritics.RemoveDiacriticsChar(p.Name.ToLower()).Contains(keyword)
                );
            }

            var totalCount = await query.CountAsync(); // tổng số sản phẩm sau lọc

            // Sắp xếp
            if (!string.IsNullOrEmpty(queryProducts.SortBy))
            {
                query = queryProducts.SortBy.ToLower() switch
                {
                    "price" => queryProducts.isDecsending
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),

                    "name" => queryProducts.isDecsending
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),

                    _ => query // nếu không khớp sortBy thì không sắp xếp
                };
            }

            // Phân trang trước khi lọc
            if (queryProducts.PageNumber > 0 && queryProducts.PageSize > 0) {
                query = query
                    .Skip((queryProducts.PageNumber - 1) * queryProducts.PageSize)
                    .Take(queryProducts.PageSize);
            }

            // Lấy dữ liệu từ DB
            var resProducts = await query.ToListAsync();


            // Ánh xạ sang DTO
            var resultProduct = resProducts.Select(resProduct => new ProductResultDto
            {
                Id = resProduct.Id,
                Name = resProduct.Name,
                Price = resProduct.Price,
                Urls = resProduct.ImageProducts?.Select(ip => ip.Url).ToList(),
                Description = resProduct.ProductDetail?.Description,
                DescriptionDetail = resProduct.ProductDetail?.GetDescriptionFromFile(),
                Quantity = resProduct.ProductDetail?.Quantity ?? 0,
                Tag = resProduct.ProductDetail?.Tag,
                Sold = resProduct.ProductDetail?.Sold ?? 0,
                Slug = resProduct.Slug,
                ExpiryDate = resProduct.ProductDetail?.ExpiryDate,
                category = resProduct.Category == null ? null : new CategoryDto
                {
                    Id = resProduct.Category.Id,
                    Name = resProduct.Category.Name,
                    Description = resProduct.Category.Description,
                    Urls = resProduct.Category.ImageCategories?.Select(ic => ic.Url).ToList()
                }
            }).ToList();

            return new PageResult<ProductResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryProducts.PageNumber,
                PageSize = queryProducts.PageSize,
                Datas = resultProduct
            };
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
                Description = resProduct?.ProductDetail?.Description,
                DescriptionDetail = resProduct?.ProductDetail?.GetDescriptionFromFile(),
                Quantity = resProduct?.ProductDetail?.Quantity ?? 0,
                Tag = resProduct?.ProductDetail?.Tag,
                Sold = resProduct?.ProductDetail?.Sold ?? 0,
                Slug = resProduct?.Slug,
                ExpiryDate = resProduct?.ProductDetail?.ExpiryDate,
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

        public async Task<int?> GetIdProductBySlug(string slug)
        {
            return await _dataContext.Products.Where(p => p.Slug == slug).Select(p => p.Id).FirstOrDefaultAsync();
        }
    }
}
