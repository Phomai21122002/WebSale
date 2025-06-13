using System.ComponentModel.DataAnnotations;
using WebSale.Models;

namespace WebSale.Dto.ProductDetails
{
    public class ProductDetailBase
    {
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? DescriptionDetail { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int? Quantity { get; set; }
        [MaxLength(255)]
        public string? Tag { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
