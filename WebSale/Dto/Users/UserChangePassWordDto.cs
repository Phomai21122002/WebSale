using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Users
{
    public class UserChangePassWordDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? Password { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? NewPassword { get; set; }
    }
}
