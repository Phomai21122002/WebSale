using WebSale.Dto.Addresses;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IUserRepository
    {
        Task<PageResult<User>> GetUsers(QueryFindSoftPaginationDto queryUsers);
        Task<User?> GetUser(string userId);
        Task<int?> TotalUser();
        Task<UserResultDto?> GetResultUser(string userId);
        Task<bool> UserExists(string userId);
        Task<User> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<User> CheckCode(string email, int code);
        Task<bool> Save();
    }
}
