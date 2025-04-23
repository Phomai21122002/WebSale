using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;

namespace WebSale.Dto.Products
{
    public class ProductResultDto : ProductDetailResultDto
    {
        public string? Slug { get; set; }
        public CategoryDto? category { get; set; }
    }
}
