using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;

namespace WebSale.Dto.Products
{
    public class ProductOrderResultDto : ProductDetailResultDto
    {
        public int? Status { get; set; }
        public CategoryDto? category { get; set; }
    }
}
