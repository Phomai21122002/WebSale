using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DataContext _dbContext;

        public LoginRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckPassword(string password)
        {
            return await _dbContext.Users.AnyAsync(u => u.Password == password);
        }

        public async Task<User> CheckUserByEmail(string email)
        {
            return await _dbContext.Users.Include(u => u.Role).Where(u => u.Email == email).SingleOrDefaultAsync();
        }
    }
}
