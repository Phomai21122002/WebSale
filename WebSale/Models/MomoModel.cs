using System.ComponentModel.DataAnnotations;

namespace WebSale.Models
{
    public class MomoModel
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string FullName { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }

        public DateTime DatePaid { get; set; }
    }
}
