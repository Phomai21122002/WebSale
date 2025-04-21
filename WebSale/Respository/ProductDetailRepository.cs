using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebSale.Respository
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ProductDetailRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ProductDetail> CreateProductDetail(ProductDetail productDetail)
        {
            await _dataContext.AddAsync(productDetail);
            await _dataContext.SaveChangesAsync();
            return productDetail;
        }

        public async Task<bool> DeleteProductDetail(ProductDetail productDetail)
        {
            _dataContext.ProductDetails.Remove(productDetail);
            return await Save();
        }

        public async Task<ProductDetail?> GetProductDetail(int id)
        {
            return await _dataContext.ProductDetails.Where(pd => pd.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<ProductDetail>> GetProductDetails(ICollection<int> productDetailsId)
        {
            return await _dataContext.ProductDetails.Where(pc => productDetailsId.Contains(pc.Id)).ToListAsync();
        }

        public async Task<bool> ProductDetailExists(int id)
        {
            return await _dataContext.ProductDetails.AnyAsync(pd => pd.Id == id);
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductDetail(ProductDetail productDetail)
        {
            _dataContext.Update(productDetail);
            return await Save();
        }

        public async Task<bool> UpdateProductDetails(ICollection<ProductDetail> productDetails)
        {
            _dataContext.UpdateRange(productDetails);
            return await Save();
        }
    }
}
