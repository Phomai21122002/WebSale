namespace WebSale.Dto.Carts
{
    public class CartDto : CartBase
    {
        public string? UserId { get; set; }
        public int ProductId { get; set; }

    }
}
