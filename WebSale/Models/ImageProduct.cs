using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class ImageProduct : Image
    {
        public Product? Product { get; set; }
    }
}
