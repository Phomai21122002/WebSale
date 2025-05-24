using WebSale.Dto.Bills;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IBillRepository
    {
        Task<BillResultDto> GetResultBillByUserId(string userId, int billId);
        Task<PageResult<BillResultDto>> GetResultsBillByUserId(string userId, QueryPaginationDto queryPaginationDto);
        Task<PageResult<BillResultDto>> GetResultsBill(QueryPaginationDto queryPaginationDto);
        Task<double?> TotalSales();
        Task<Bill> CreateBill(Bill bill);
        Task<bool> BillExists(string userId, int billId);
    }
}
