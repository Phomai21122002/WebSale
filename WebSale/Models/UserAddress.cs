namespace WebSale.Models
{
    public class UserAddress : BaseTime
    {
        public string? UserId { get; set; }
        public int AddressId { get; set; }
        public bool IsDefault { get; set; }
        public User? User { get; set; }
        public Address? Address { get; set; }
    }
}
