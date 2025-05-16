using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class BillDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be non-negative")]
        public long Price { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? DescriptionDetail { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }
        public Bill Bill { get; set; }
    }
}
