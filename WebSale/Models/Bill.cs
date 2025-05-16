using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Bill : BaseTime
    {
        [Key]
        public int Id { get; set; }
        public string? PaymentMethod { get; set; }
        public User? User { get; set; }
        public ICollection<BillDetail>? BillDetails { get; set; }
        public VnpayModel? Vnpay { get; set; }
    }
}
