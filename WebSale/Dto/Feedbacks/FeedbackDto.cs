using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Feedbacks
{
    public class FeedbackDto
    {
        public int ProductId { get; set; }
        [Required]
        [MaxLength(1000)]
        public string? Content { get; set; }
        [Required]
        public int Rate { get; set; }
        [Required]
        public List<string>? Urls { get; set; }
    }
}
