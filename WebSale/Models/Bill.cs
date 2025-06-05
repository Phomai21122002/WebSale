using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Bill : BaseTime
    {
        [Key]
        public int Id { get; set; }
        public string? NameOrder { get; set; }
        public string? PaymentMethod { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }
        [Required]
        [StringLength(200)]
        public string? FullName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string? Phone { get; set; }
        public User? User { get; set; }
        public ICollection<BillDetail>? BillDetails { get; set; }
        public VnpayModel? Vnpay { get; set; }
    }
}
