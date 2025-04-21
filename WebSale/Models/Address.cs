using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Address : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        public long Code { get; set; }
        public ICollection<UserAddress>? UserAddresses {  get; set; }
    }
}
