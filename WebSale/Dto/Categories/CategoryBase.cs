using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Categories
{
    public class CategoryBase
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public List<string>? Urls { get; set; }
    }
}
