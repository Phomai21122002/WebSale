using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Users
{
    public class UserBaseDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string? FirstName { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        public string? LastName { get; set; }
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string? Phone { get; set; }
        public string? url { get; set; }

    }
}
