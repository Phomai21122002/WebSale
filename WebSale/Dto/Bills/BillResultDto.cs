using WebSale.Dto.Addresses;

namespace WebSale.Dto.Bills
{
    public class BillResultDto
    {
        public int Id { get; set; }
        public string? NameOrder { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public string? PaymentMethod { get; set; }
        public int? QuantityProduct { get; set; }
        public int? Total { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsCompleted { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public ICollection<BillDetailResultDto>? BillDetails { get; set; }
        public UserResultDto? User { get; set; }
    }
}
