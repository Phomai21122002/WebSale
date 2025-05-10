using WebSale.Models;
using WebSale.Models.Vnpay;

namespace WebSale.Services.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
        Task<VnpayModel> CreateVnPayModel(VnpayModel model);

    }
}
