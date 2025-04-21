using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;

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

        public async Task<ICollection<Product>> GetProducts()
        {
            return await _dataContext.Products.Include(p => p.ImageProducts).ToListAsync();
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

    }
}
