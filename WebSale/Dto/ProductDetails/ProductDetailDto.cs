using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.ProductDetails
{
    public class ProductDetailDto : ProductDetailBase
    {
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be non-negative")]
        public long Price { get; set; }
        [Required]
        public List<string>? Urls { get; set; }
    }
}
