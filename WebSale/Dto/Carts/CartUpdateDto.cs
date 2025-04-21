namespace WebSale.Dto.Carts
{
    public class CartUpdateDto : CartBase
    {
        public string? UserId { get; set; }
        public int CartId { get; set; }
    }
}
