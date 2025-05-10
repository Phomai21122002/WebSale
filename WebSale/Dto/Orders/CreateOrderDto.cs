namespace WebSale.Dto.Orders
{
    public class CreateOrderDto
    {
        public OrderPaymentStatus PaymentMethod { get; set; }
        public ICollection<int>? CartsId { get; set; }
    }
}
