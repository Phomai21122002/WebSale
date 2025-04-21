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
        [Required]
        [Range(1000000000, 9999999999, ErrorMessage = "Phone must be a valid 10-digit number.")]
        public long? Phone { get; set; }
    }
}
