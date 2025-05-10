namespace WebSale.Dto.Carts
{
    public class CartUpdateOrderDto
    {
        public string? UserId { get; set; }
        public ICollection<int> CartsId { get; set; }
    }
}
