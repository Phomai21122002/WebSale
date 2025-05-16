using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Users
{
    public class UserUpdateDto : UserBaseDto
    {
        [Required]
        public int IdRole { get; set; }
    }
}
