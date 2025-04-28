using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IAddressRepository
    {
        Task<ICollection<Address>> GetAddressesByUserId(string userId);
        Task<Address> GetAddress(string userId, int addressId);
        Task<Address> CreateAddress(Address address);
        Task<Address> UpdateAddress(Address address);
        Task<Address> DeleteAddress(Address address);
        Task<ICollection<Provinces>> GetProvinces();
        Task<Provinces> GetProvinceByCode(string code);
        Task<ICollection<District>> GetDistricstByParentCode(string parentCode);
        Task<ICollection<Ward>> GetWardsByParentCode(string parentCode);
        Task<bool> AddressExists(string userId, int addressId);
        Task<bool> Save();
    }
}
