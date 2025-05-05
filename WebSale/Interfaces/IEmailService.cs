using WebSale.Dto.Email;

namespace WebSale.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
