using O2OBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<int> AddAsync(Role role);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
        Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
    }
}