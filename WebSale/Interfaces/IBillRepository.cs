using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> GetResultBillByUserId(string userId, int billId);
        Task<ICollection<Bill>> GetResultsBillByUserId(string userId);
        Task<ICollection<Bill>> GetResultsBill();
        Task<Bill> CreateBill(Bill bill);
        Task<bool> BillExists(string userId, int billId);
    }
}
