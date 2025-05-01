using System.ComponentModel.DataAnnotations;
using WebSale.Models;

namespace WebSale.Dto.Categories
{
    public class CategoryResultDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CountProduct { get; set; }
        public ICollection<ImageCategory>? ImageCategories { get; set; }
    }
}
