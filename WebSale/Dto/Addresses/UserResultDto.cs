using System.ComponentModel.DataAnnotations;
using WebSale.Models;

namespace WebSale.Dto.Addresses
{
    public class UserResultDto
    {
        [Key]
        [StringLength(255)]
        public string? Id { get; set; }
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
        public string? Url { get; set; }
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string? Phone { get; set; }
        public Role? Role { get; set; }
        public ICollection<AddressDto>? Addresses { get; set; }
    }
}
