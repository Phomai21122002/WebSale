using WebSale.Dto.Bills;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IBillRepository
    {
        Task<ICollection<Bill>> GetBills(string userId);
        Task<BillResultDto> GetResultBillByUserId(string userId, int billId);
        Task<PageResult<BillResultDto>> GetResultsBillByUserId(string userId, QueryPaginationDto queryPaginationDto);
        Task<PageResult<BillResultDto>> GetResultsBill(QueryFindPaginationDto queryFindPaginationDto);
        Task<double?> TotalSales();
        Task<Bill> CreateBill(Bill bill);
        Task<bool> UpdateBill(Bill bill);
        Task<bool> BillExists(string userId, int billId);
    }
}
