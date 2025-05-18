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

namespace WebSale.Respository
{
    public class BillRepository : IBillRepository
    {
        private readonly DataContext _dataContext;

        public BillRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> BillExists(string userId, int billId)
        {
            return await _dataContext.Bills.AnyAsync(o => o.Id == billId && o.User != null && o.User.Id == userId);
        }

        public async Task<Bill> CreateBill(Bill bill)
        {
            await _dataContext.AddAsync(bill);
            await _dataContext.SaveChangesAsync();
            return bill;
        }

        public async Task<Bill> GetResultBillByUserId(string userId, int billId)
        {
            return await _dataContext.Bills.Where(b => b.Id == billId && b.User != null && b.User.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Bill>> GetResultsBill()
        {
            return await _dataContext.Bills.ToListAsync();
        }

        public async Task<ICollection<Bill>> GetResultsBillByUserId(string userId)
        {
            return await _dataContext.Bills.Where(b => b.User != null && b.User.Id == userId).ToListAsync();
        }
    }
}
