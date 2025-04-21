using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class ImageFeedBack : Image
    {
        public FeedBack? FeedBack { get; set; }
    }
}
