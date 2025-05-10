using WebSale.Dto.Addresses;
using WebSale.Dto.Users;

namespace WebSale.Dto.Orders
{
    public class OrderResultDto : OrderDto
    {
        public int CountProduct { get; set; }
        public UserResultDto? User { get; set; }
    }
}
