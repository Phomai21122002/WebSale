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
        [Phone]
        public long? Phone {  get; set; }
        public ICollection<UserAddress>? UserAddresses { get; set; }
        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<FeedBack>? FeedBacks { get; set; }
        public Role? Role { get; set; }

        internal static string FindFirst(string nameIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
