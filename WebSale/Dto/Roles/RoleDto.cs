using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Roles
{
    public class RoleDto
    {
        [Required]
        public string? Name { get; set; }
    }
}
