using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Dto.Addresses;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class AddressRepository : IAddressRepository
    {

        private readonly DataContext _dataContext;

        public AddressRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> AddressExists(string userId, int addressId)
        {
            return await _dataContext.UserAddresses.AnyAsync(a => a.AddressId == addressId && a.UserId == userId);
        }

        public async Task<Address> CreateAddress(Address address)
        {
            await _dataContext.AddAsync(address);
            return await Save() ? address : null;
        }

        public async Task<Address> DeleteAddress(Address address)
        {
            _dataContext.Remove(address);
            return await Save() ? address : null;
        }

        public async Task<Address?> GetAddress(string userId, int addressId)
        {
            return await _dataContext.Addresses.Where(a => a.Id == addressId && a.UserAddresses.Any(ua => ua.UserId == userId)).FirstOrDefaultAsync();
        }

        public async Task<ICollection<AddressDto>> GetAddressesByUserId(string userId)
        {
            return await _dataContext.Addresses
                .Where(a => a.UserAddresses.Any(ua => ua.UserId == userId))
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    IsDefault = a.UserAddresses
                        .Where(ua => ua.UserId == userId)
                        .Select(ua => ua.IsDefault)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<ICollection<District>> GetDistricstByParentCode(string parentCode)
        {
            return await _dataContext.Districts.Where(d => d.ParentCode == parentCode).ToListAsync();
        }

        public async Task<Provinces?> GetProvinceByCode(string code)
        {
            return await _dataContext.Provinces.Where(p=> p.Code == code).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Provinces>> GetProvinces()
        {
            return await _dataContext.Provinces.ToListAsync();
        }

        public async Task<ICollection<Ward>> GetWardsByParentCode(string parentCode)
        {
            return await _dataContext.Wards.Where(w => w.ParentCode == parentCode).ToListAsync();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Address> UpdateAddress(Address address)
        {
            _dataContext.Update(address);
            return await Save() ? address : null;
        }
    }
}
