using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class ProductDetail : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? DescriptionDetail { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }
        [MaxLength(255)]
        public string? Tag { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sold must be non-negative")]
        public int Sold { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Product? Product { get; set; }
    }
}
