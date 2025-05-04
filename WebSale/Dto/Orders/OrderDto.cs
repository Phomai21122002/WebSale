using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Orders
{
    public class OrderDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        public int? Status { get; set; }
        [Required]
        [Range(0, long.MaxValue)]
        public long Total { get; set; }
        public DateTime? CreateOrder { get; set; }
    }
}
