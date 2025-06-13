namespace WebSale.Dto.Carts
{
    public class CartDeleteDto
    {
        public string? UserId { get; set; }
        public ICollection<int>? CartsId { get; set; }
    }
}
