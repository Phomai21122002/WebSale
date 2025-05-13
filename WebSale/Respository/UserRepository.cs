using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Dto.Addresses;
using WebSale.Dto.QueryDto;
using WebSale.Extensions;
using WebSale.Interfaces;
using WebSale.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<PageResult<User>> GetUsers(QueryFindSoftPaginationDto queryUsers)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            var totalCount = await query.CountAsync();

            // Sắp xếp
            if (!string.IsNullOrEmpty(queryUsers.SortBy))
            {
                query = queryUsers.SortBy.ToLower() switch
                {
                    "email" => queryUsers.isDecsending
                        ? query.OrderByDescending(p => p.Email)
                        : query.OrderBy(p => p.Email),
                    "name" => queryUsers.isDecsending
                        ? query.OrderByDescending(p => p.FirstName)
                    : query.OrderBy(p => p.FirstName),
                    "phone" => queryUsers.isDecsending
                        ? query.OrderByDescending(p => p.Phone)
                    : query.OrderBy(p => p.Phone),

                    _ => query
                };
            }

            // Phân trang trước khi lọc
            if (queryUsers.PageNumber > 0 && queryUsers.PageSize > 0)
            {
                query = query
                    .Skip((queryUsers.PageNumber - 1) * queryUsers.PageSize)
                    .Take(queryUsers.PageSize);
            }

            // Lấy dữ liệu từ DB
            var resUsers = await query.ToListAsync();

            if (!string.IsNullOrEmpty(queryUsers.Name))
            {
                var keyword = RemoveDiacritics.RemoveDiacriticsChar(queryUsers.Name.ToLower());

                resUsers = resUsers.Where(p =>
                    (!string.IsNullOrEmpty(p.FirstName) &&
                     RemoveDiacritics.RemoveDiacriticsChar(p.FirstName.ToLower()).Contains(keyword))
                    ||
                    (!string.IsNullOrEmpty(p.LastName) &&
                     RemoveDiacritics.RemoveDiacriticsChar(p.LastName.ToLower()).Contains(keyword))
                ).ToList();
            }


            return new PageResult<User>
            {
                TotalCount = totalCount,
                PageNumber = queryUsers.PageNumber,
                PageSize = queryUsers.PageSize,
                Datas = resUsers
            };
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
