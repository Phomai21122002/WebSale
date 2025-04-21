using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IRoleRepository
    {
        ICollection<Role> GetRoles();
        Role GetRole(int roleId);
        bool CreateRole(Role role);
        bool UpdateRole(Role role);
        bool DeleteRole(Role role);
        bool Save();
        bool RoleExists(string nameRole);
        bool RoleIdExists(int idRole);
    }
}
