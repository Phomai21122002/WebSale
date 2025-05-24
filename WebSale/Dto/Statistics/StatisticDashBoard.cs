using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Bills
{
    public class StatisticDashBoard
    {
        public int? TotalUsers { get; set; }
        public int? TotalOrders { get; set; }
        public int? TotalProducts { get; set; }
        public int? TotalCategories { get; set; }
        public double? TotalSales { get; set; }
    }
}
