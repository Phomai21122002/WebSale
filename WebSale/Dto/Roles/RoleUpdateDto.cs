using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Roles
{
    public class RoleUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
