using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class ImageCategory : Image
    {
        public Category? Category { get; set; }
    }
}
