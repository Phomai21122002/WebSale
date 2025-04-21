using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Role : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
    }
}
