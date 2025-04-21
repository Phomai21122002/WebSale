using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Users;

namespace WebSale.Dto.Auth
{
    public class RegisterDto : UserBaseDto
    {

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? Password { get; set; }
    }
}
