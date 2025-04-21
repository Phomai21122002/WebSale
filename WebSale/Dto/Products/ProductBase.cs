using System.ComponentModel.DataAnnotations;
using WebSale.Models;

namespace WebSale.Dto.Products
{
    public class ProductBase
    {
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be non-negative")]
        public long Price { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Slug { get; set; }
    }
}
