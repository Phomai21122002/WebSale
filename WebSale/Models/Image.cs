using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Image : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Url { get; set; }
    }
}
