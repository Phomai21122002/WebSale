using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IBillDetailRepository
    {
        Task<ICollection<BillDetail>> CreateBillDetail(ICollection<BillDetail> billDetails);
    }
}
