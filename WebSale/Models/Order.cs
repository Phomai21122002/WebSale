using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Order : BaseTime
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
        public string PaymentMethod { get; set; }
        public bool IsPayment {  get; set; } = false;
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public User? User { get; set; }
        public VnpayModel? Vnpay { get; set; }
    }
}
