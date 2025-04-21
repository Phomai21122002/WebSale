using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Categories
{
    public class CategoryDto : CategoryBase
    {
        [Required]
        public int Id { get; set; }
    }
}
