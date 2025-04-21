using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Cart : BaseTime
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        public User? User { get; set; }
        public Product? Product { get; set; }

    }
}
