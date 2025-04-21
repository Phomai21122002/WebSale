using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface ILoginRepository
    {
        Task<User> CheckUserByEmail(string email);
        Task<bool> CheckPassword(string password);
    }
}
