using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;

namespace WebSale.Dto.Carts
{
    public class CartDetailDto
    {
        public int Id { get; set; }
        public long Total { get; set; }
        public ProductDetailResultDto? products { get; set; }
        public CategoryDto? category { get; set; }
    }
}
