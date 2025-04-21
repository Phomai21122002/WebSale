using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateUser(User user)
        {
            await  _context.AddAsync(user);
            return await Save();
        }

        public async Task<bool> DeleteUser(User user)
        {
            _context.Remove(user);
            return await Save();
        }

        public async Task<User?> GetUser(string userId)
        {
            return await _context.Users.Include(u => u.Role).Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<User>> GetUsers()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
            // sap xep va phan trang
            //.OrderBy(s => s.Name)
            //.Skip((pageNumber - 1) * pageSize)
            //.Take(pageSize)
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Update(user);
            return await Save();
        }

        public async Task<bool> UserExists(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
