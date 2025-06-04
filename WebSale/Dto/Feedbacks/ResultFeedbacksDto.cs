using System.ComponentModel.DataAnnotations;
using WebSale.Models;

namespace WebSale.Dto.Feedbacks
{
    public class ResultFeedbacksDto
    {
        [Required]
        public int StarOne { get; set; }
        public int StarTwo { get; set; }
        public int StarThree { get; set; }
        public int StarFour { get; set; }
        public int StarFive { get; set; }
        public float StarAverage { get; set; }
        public ICollection<FeedBack> feedBacks { get; set; }
    }
}
