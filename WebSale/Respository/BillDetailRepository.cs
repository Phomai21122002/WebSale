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
    }
}
