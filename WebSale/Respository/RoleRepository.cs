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
        public bool CreateRole(Role role)
        {
            _context.Add(role);
            return Save();
        }

        public Role GetRole(int roleId)
        {
            return _context.Roles.Where(r => r.Id ==  roleId).FirstOrDefault();
        }

        public ICollection<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public bool Save()
        {
            var role = _context.SaveChanges();
            return role > 0 ? true : false;
        }

        public bool RoleExists(string nameRole) {
            return _context.Roles.Any(r => r.Name.Trim().ToLower() == nameRole.Trim().ToLower());
        }

        public bool RoleIdExists(int idRole)
        {
            return _context.Roles.Any(r => r.Id == idRole);
        }

        public bool UpdateRole(Role role)
        {
            _context.Update(role);
            return Save();
        }

        public bool DeleteRole(Role role)
        {
            _context.Remove(role);
            return Save();
        }
    }
}
