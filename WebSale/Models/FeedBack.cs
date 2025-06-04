using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class FeedBack : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(1000)]
        public string? Content { get; set; }
        public int Rate { get; set; }
        public ICollection<ImageFeedBack>? ImageFeedBacks { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}
