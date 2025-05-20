using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class Category : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<ImageCategory>? ImageCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
