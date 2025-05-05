using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Dto.Addresses;
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
            return await _context.Users.Include(u => u.Role).Include(u => u.UserAddresses.Where(ud => ud.IsDefault)).Where(u => u.Id == userId).FirstOrDefaultAsync();
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

        public async Task<UserResultDto?> GetResultUser(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserResultDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Phone = u.Phone,
                    url = u.url,
                    Addresses = u.UserAddresses!
                        .Select(ud => new AddressDto
                        {
                            Id = ud.Address!.Id,
                            Name = ud.Address!.Name!,
                            Code = ud.Address.Code,
                            IsDefault = ud.IsDefault
                        })
                        .ToList(),
                    Role = u.Role
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> CheckCode(string email, int code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Code == code);
            if (user == null || user.ConfirmEmail)
                return null;
            return user;
        }
    }
}
