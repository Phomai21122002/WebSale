using WebSale.Models.Momo;

namespace WebSale.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
