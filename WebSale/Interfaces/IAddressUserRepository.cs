using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IAddressUserRepository
    {
        Task<ICollection<UserAddress>> GetAllUserAddress(string userId);
        Task<bool> CreateUserAddress(UserAddress userAddress);
        Task<bool> UpdateAllUserAddress(ICollection<UserAddress> userAddresses);
        Task<UserAddress> UpdateUserAddress(UserAddress userAddress);
        Task<UserAddress> GetUserAddress(string userId, int addressId);
        Task<bool> UserAddressExists(string userId, int addressId);
        Task<bool> Save();
    }
}
