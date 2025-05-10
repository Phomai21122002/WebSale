namespace WebSale.Dto.Carts
{
    public class CartUpdateDto : CartBase
    {
        public bool IsSelectedForOrder { get; set; }
        public string? UserId { get; set; }
        public int CartId { get; set; }
    }
}
