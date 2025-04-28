using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class AddressUserRepository : IAddressUserRepository
    {

        private readonly DataContext _dataContext;

        public AddressUserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreateUserAddress(UserAddress userAddress)
        {
            await _dataContext.AddAsync(userAddress);
            return await Save();
        }

        public async Task<ICollection<UserAddress>> GetAllUserAddress(string userId)
        {
            return await _dataContext.UserAddresses.Where(ud => ud.UserId == userId).ToListAsync();
        }

        public async Task<UserAddress?> GetUserAddress(string userId, int addressId)
        {
            return await _dataContext.UserAddresses.FirstOrDefaultAsync(ud => ud.UserId == userId && ud.AddressId == addressId);
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAllUserAddress(ICollection<UserAddress> userAddresses)
        {
            _dataContext.UpdateRange(userAddresses);
            return await Save();
        }

        public async Task<UserAddress> UpdateUserAddress(UserAddress userAddress)
        {
            _dataContext.Update(userAddress);
            return await Save() ? userAddress : null;
        }

        public async Task<bool> UserAddressExists(string userId, int addressId)
        {
            return await _dataContext.UserAddresses.AnyAsync(ud => ud.UserId == userId && ud.AddressId == addressId);
        }
    }
}
