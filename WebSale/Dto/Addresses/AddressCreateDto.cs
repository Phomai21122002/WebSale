using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Addresses
{
    public class AddressCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        public long Code { get; set; }

    }
}
