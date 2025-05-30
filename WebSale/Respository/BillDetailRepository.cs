using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebSale.Dto.Orders;
using WebSale.Dto.Addresses;
using WebSale.Dto.Products;
using WebSale.Dto.Categories;
using WebSale.Dto.Users;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSale.Extensions;
using WebSale.Dto.QueryDto;
using WebSale.Dto.Statistics;

namespace WebSale.Respository
{
    public class BillDetailRepository : IBillDetailRepository
    {
        private readonly DataContext _dataContext;

        public BillDetailRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ICollection<BillDetail>> CreateBillDetail(ICollection<BillDetail> billDetails)
        {
            await _dataContext.AddRangeAsync(billDetails);
            await _dataContext.SaveChangesAsync();
            return billDetails;
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            var monthlyRevenue = await _dataContext.BillDetails
                .Include(bd => bd.Bill)
                .Where(bd => bd.Bill != null)
                .GroupBy(bd => new
                {
                    Year = bd.Bill.CreatedAt.Value.Year,
                    Month = bd.Bill.CreatedAt.Value.Month
                })
                .Select(g => new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            return monthlyRevenue;
        }
    }
}
