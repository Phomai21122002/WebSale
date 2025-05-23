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
using WebSale.Dto.QueryDto;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<PageResult<ProductResultDto>> GetProducts(QueryFindSoftPaginationDto queryProducts)
        {
            var baseQuery = _dataContext.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.ImageProducts)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ImageCategories)
                .Include(p => p.ProductDetail);

            var resProducts = await baseQuery.ToListAsync();

            if (!string.IsNullOrEmpty(queryProducts.Name))
            {
                var keyword = RemoveDiacritics.RemoveDiacriticsChar(queryProducts.Name.ToLower());
                resProducts = resProducts
                    .Where(p =>
                        !string.IsNullOrEmpty(p.Name) &&
                        RemoveDiacritics.RemoveDiacriticsChar(p.Name.ToLower()).Contains(keyword)
                    )
                    .ToList();
            }

            var totalCount = resProducts.Count;

            if (!string.IsNullOrEmpty(queryProducts.SortBy))
            {
                resProducts = queryProducts.SortBy.ToLower() switch
                {
                    "price" => queryProducts.isDecsending
                        ? resProducts.OrderByDescending(p => p.Price).ToList()
                        : resProducts.OrderBy(p => p.Price).ToList(),

                    "name" => queryProducts.isDecsending
                        ? resProducts.OrderByDescending(p => p.Name).ToList()
                        : resProducts.OrderBy(p => p.Name).ToList(),

                    _ => resProducts
                };
            }

            if (queryProducts.PageNumber > 0 && queryProducts.PageSize > 0)
            {
                resProducts = resProducts
                    .Skip((queryProducts.PageNumber - 1) * queryProducts.PageSize)
                    .Take(queryProducts.PageSize)
                    .ToList();
            }

            var resultProduct = resProducts.Select(p => new ProductResultDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Urls = p.ImageProducts?.Select(ip => ip.Url).ToList(),
                Description = p.ProductDetail?.Description,
                DescriptionDetail = p.ProductDetail?.GetDescriptionFromFile(),
                Quantity = p.ProductDetail?.Quantity ?? 0,
                Tag = p.ProductDetail?.Tag,
                Sold = p.ProductDetail?.Sold ?? 0,
                Slug = p.Slug,
                ExpiryDate = p.ProductDetail?.ExpiryDate,
                IsDeleted = p.IsDeleted,
                category = p.Category == null ? null : new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description,
                    Urls = p.Category.ImageCategories?.Select(ic => ic.Url).ToList()
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


        public async Task<PageResult<ProductResultDto>> GetProductsByIdCategory(int categoryId, QuerySoftPaginationDto queryProducts)
        {
            var query = _dataContext.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.ImageProducts)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ImageCategories)
                .Include(p => p.ProductDetail)
                .Where(p => p.Category.Id == categoryId)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            query = queryProducts.isDecsending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price);

            if (queryProducts.PageNumber > 0 && queryProducts.PageSize > 0)
            {
                query = query
                    .Skip((queryProducts.PageNumber - 1) * queryProducts.PageSize)
                    .Take(queryProducts.PageSize);
            }

            var resProducts = await query.ToListAsync();

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
                IsDeleted = resProduct.IsDeleted,
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
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
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
            return await _dataContext.Products.Where(p => p.Slug == slug && !p.IsDeleted).Select(p => p.Id).FirstOrDefaultAsync();
        }
    }
}
