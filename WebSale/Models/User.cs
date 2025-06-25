using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class User : BaseTime
    {
        [Key]
        [StringLength(255)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }
        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string? Password { get; set; }
        [Required]
        public string? Url { get; set; }
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string? Phone { get; set; }
        public bool ConfirmEmail { get; set; } = false;
        [Range(100000, 999999, ErrorMessage = "Code phải là số có 6 chữ số")]
        public int Code { get; set; }
        [StringLength(255)]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<UserAddress>? UserAddresses { get; set; }
        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<FeedBack>? FeedBacks { get; set; }
        public ICollection<Bill>? Bills { get; set; }
        public Role? Role { get; set; }

    }
}
