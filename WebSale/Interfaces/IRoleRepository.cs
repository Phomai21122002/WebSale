using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IRoleRepository
    {
        Task<ICollection<Role>> GetRoles();
        Task<Role> GetRole(int roleId);
        Task<bool> CreateRole(Role role);
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<bool> Save();
        Task<bool> RoleExists(string nameRole);
        Task<bool> RoleIdExists(int idRole);
    }
}
