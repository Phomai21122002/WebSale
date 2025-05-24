namespace WebSale.Dto.Orders
{
    public class OrderResultStatisticDto
    {
        public int AmountPending { get; set; }
        public int AmountShipped { get; set; }
        public int AmountRecieved { get; set; }
        public int AmountCancelled { get; set; }
    }
}
