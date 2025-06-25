using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Feedbacks
{
    public class ResultRatingsDto
    {
        public int ProductId { get; set; }
        [Required]
        public string UserId { get; set; }
        public double Rating { get; set; }
    }
}
