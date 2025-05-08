using WebSale.Dto.Users;

namespace WebSale.Dto.Orders
{
    public class OrderResultDto : OrderDto
    {
        public int CountProduct { get; set; }
        public UserDto? User { get; set; }
    }
}
