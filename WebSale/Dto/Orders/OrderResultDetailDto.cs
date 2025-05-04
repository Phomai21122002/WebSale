using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Addresses;
using WebSale.Dto.Products;

namespace WebSale.Dto.Orders
{
    public class OrderResultDetailDto : OrderDto
    {
        public int CountProduct { get; set; }
        public ICollection<ProductOrderResultDto>? Products { get; set; }
        public UserResultDto? User { get; set; }
    }
}
