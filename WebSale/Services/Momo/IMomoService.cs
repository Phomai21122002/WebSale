using WebSale.Models.Momo;

namespace WebSale.Services.Momo
{
    public interface IMomoService
    {
        Task<string> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
