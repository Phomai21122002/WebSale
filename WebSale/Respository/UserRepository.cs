﻿using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async Task<User> CreateUser(User user)
        {
            await  _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
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

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.UserAddresses.Where(ud => ud.IsDefault)).Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<PageResult<UserResultDto>> GetUsers(QueryFindSoftPaginationDto queryUsers)
        {
            var query = _context.Users
                            .Include(u => u.Role)
                            .Include(u => u.UserAddresses)
                                .ThenInclude(ua => ua.Address);
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
            var totalCount = resUsers.Count;

            // Sắp xếp
            if (!string.IsNullOrEmpty(queryUsers.SortBy))
            {
                resUsers = queryUsers.SortBy.ToLower() switch
                {
                    "email" => queryUsers.isDecsending
                        ? resUsers.OrderByDescending(p => p.Email).ToList()
                        : resUsers.OrderBy(p => p.Email).ToList(),
                    "name" => queryUsers.isDecsending
                        ? resUsers.OrderByDescending(p => p.FirstName).ToList()
                    : resUsers.OrderBy(p => p.FirstName).ToList(),
                    "phone" => queryUsers.isDecsending
                        ? resUsers.OrderByDescending(p => p.Phone).ToList()
                    : resUsers.OrderBy(p => p.Phone).ToList(),

                    _ => resUsers
                };
            }

            // Phân trang trước khi lọc
            if (queryUsers.PageNumber > 0 && queryUsers.PageSize > 0)
            {
                resUsers = resUsers
                    .Skip((queryUsers.PageNumber - 1) * queryUsers.PageSize)
                    .Take(queryUsers.PageSize).ToList();
            }

            var resultUsers = resUsers.Select(u => new UserResultDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Phone = u.Phone,
                Url = u.Url,
                Addresses = u.UserAddresses?.Where(ud => ud.Address != null)
                        .Select(ud => new AddressDto
                        {
                            Id = ud.Address!.Id,
                            Name = ud.Address!.Name!,
                            Code = ud.Address.Code,
                            IsDefault = ud.IsDefault
                        })
                        .ToList(),
                Role = u.Role
            }).ToList();

            return new PageResult<UserResultDto>
            {
                TotalCount = totalCount,
                PageNumber = queryUsers.PageNumber,
                PageSize = queryUsers.PageSize,
                Datas = resultUsers
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
                    Url = u.Url,
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
            return user;
        }

        public async Task<int?> TotalUser()
        {
            int count = await _context.Users.CountAsync();
            return count;
        }
    }
}
