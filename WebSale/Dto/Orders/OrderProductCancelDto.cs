using WebSale.Dto.Addresses;
using WebSale.Dto.Products;

namespace WebSale.Dto.Orders
{
    public class OrderProductCancelDto : OrderDto
    {
        public ProductOrderResultDto? Product { get; set; }
        public UserResultDto? User { get; set; }
    }
}
