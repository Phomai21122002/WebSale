using Microsoft.EntityFrameworkCore;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly DataContext _context;
        public RoleRepository(DataContext context) {
            _context = context;
        }
        public async Task<bool> CreateRole(Role role)
        {
            await _context.AddAsync(role);
            return await Save();
        }

        public async Task<Role> GetRole(int roleId)
        {
            return await _context.Roles.Where(r => r.Id ==  roleId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Role>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RoleExists(string nameRole) {
            return await _context.Roles.AnyAsync(r => r.Name.Trim().ToLower() == nameRole.Trim().ToLower());
        }

        public async Task<bool> RoleIdExists(int idRole)
        {
            return await _context.Roles.AnyAsync(r => r.Id == idRole);
        }

        public async Task<bool> UpdateRole(Role role)
        {
            _context.Update(role);
            return await Save();
        }

        public Task<bool> DeleteRole(Role role)
        {
            _context.Remove(role);
            return Save();
        }
    }
}
