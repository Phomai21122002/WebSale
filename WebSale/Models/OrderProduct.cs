using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class OrderProduct : BaseTime
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        [Required]
        public int? Status { get; set; }
        public Product? Product { get; set; }
        public Order? Order { get; set; }
    }
}
