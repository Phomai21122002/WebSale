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
using WebSale.Dto.Bills;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public async Task<BillResultDto> GetResultBillByUserId(string userId, int billId)
        {
            var bill = await _dataContext.Bills
                .Include(b => b.User)
                    .ThenInclude(ua => ua.UserAddresses)
                        .ThenInclude(a => a.Address)
                .Include(b => b.BillDetails)
                .Where(b => b.Id == billId && b.User != null && b.User.Id == userId)
                .FirstOrDefaultAsync();
            var resultBill = new BillResultDto
            {
                Id = bill.Id,
                NameOrder = bill.NameOrder,
                PaymentMethod = bill.PaymentMethod,
                QuantityProduct = bill.BillDetails.Count,
                Total = bill.BillDetails.Sum(bd => bd.Quantity),
                TotalPrice = bill.BillDetails.Sum(bd => bd.Quantity * bd.Price),
                IsCompleted = true,
                CreatedAt = bill.CreatedAt,
                BillDetails = bill.BillDetails.Select(bd => new BillDetailResultDto
                {
                    Id = bd.Id,
                    Name = bd.Name,
                    Price = bd.Price,
                    Slug = bd.Slug,
                    Description = bd.Description,
                    DescriptionDetail = bd.DescriptionDetail,
                    Quantity = bd.Quantity,
                }).ToList(),
                User = new UserResultDto
                {
                    Id = bill.User?.Id,
                    Email = bill.User?.Email,
                    FirstName = bill.User?.FirstName,
                    LastName = bill.User?.LastName,
                    Phone = bill.User?.Phone,
                    url = bill.User?.url,
                    Addresses = bill.User?.UserAddresses!
                        .Select(ud => new AddressDto
                        {
                            Id = ud.Address!.Id,
                            Name = ud.Address!.Name!,
                            Code = ud.Address.Code,
                            IsDefault = ud.IsDefault
                        })
                        .ToList(),
                }
            };
            return resultBill;
        }

        public async Task<PageResult<BillResultDto>> GetResultsBill(QueryPaginationDto queryPaginationDto)
        {
            var query = _dataContext.Bills
                .Include(b => b.User)
                .Include(b=>b.BillDetails)
                .AsQueryable();

            var totalCount = await query.CountAsync();


            if (queryPaginationDto.PageNumber > 0 && queryPaginationDto.PageSize > 0)
            {
                query = query
                    .Skip((queryPaginationDto.PageNumber - 1) * queryPaginationDto.PageSize)
                    .Take(queryPaginationDto.PageSize);
            }

            var bills = await query.ToListAsync();

            var resultBills = bills.Select(b => new BillResultDto {
                Id = b.Id,
                NameOrder = b.NameOrder,
                PaymentMethod = b.PaymentMethod,
                QuantityProduct = b.BillDetails.Count,
                Total = b.BillDetails.Sum(bd => bd.Quantity),
                TotalPrice = b.BillDetails.Sum(bd => bd.Quantity * bd.Price),
                IsCompleted = true
            }).ToList();

            return new PageResult<BillResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryPaginationDto.PageNumber,
                PageSize = queryPaginationDto.PageSize,
                Datas = resultBills
            };
        }

        public async Task<PageResult<BillResultDto>> GetResultsBillByUserId(string userId, QueryPaginationDto queryPaginationDto)
        {
            var query = _dataContext.Bills
                .Where(b => b.User != null && b.User.Id == userId)
                .Include(b => b.User)
                .Include(b=>b.BillDetails)
                .AsQueryable();

            var totalCount = await query.CountAsync();


            if (queryPaginationDto.PageNumber > 0 && queryPaginationDto.PageSize > 0)
            {
                query = query
                    .Skip((queryPaginationDto.PageNumber - 1) * queryPaginationDto.PageSize)
                    .Take(queryPaginationDto.PageSize);
            }

            var bills = await query.ToListAsync();

            var resultBills = bills.Select(b => new BillResultDto
            {
                Id = b.Id,
                NameOrder = b.NameOrder,
                PaymentMethod = b.PaymentMethod,
                QuantityProduct = b.BillDetails.Count,
                Total = b.BillDetails.Sum(bd => bd.Quantity),
                TotalPrice = b.BillDetails.Sum(bd => bd.Quantity * bd.Price),
                IsCompleted = true
            }).ToList();

            return new PageResult<BillResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryPaginationDto.PageNumber,
                PageSize = queryPaginationDto.PageSize,
                Datas = resultBills
            };

        }
    }
}
