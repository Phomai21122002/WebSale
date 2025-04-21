namespace WebSale.Dto.Orders
{
    public class CreateOrderDto
    {
        public ICollection<int>? CartsId { get; set; }
    }
}
