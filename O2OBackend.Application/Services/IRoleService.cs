using O2OBackend.Application.DTOs.Role;
using O2OBackend.Application.DTOs.Permission; // For GetPermissionsForRoleAsync
using O2OBackend.Application.DTOs.User;       // For GetUsersInRoleAsync
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Application.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleByNameAsync(string name);
        Task<int> CreateRoleAsync(RoleCreateDto roleCreateDto);
        Task<bool> UpdateRoleAsync(RoleUpdateDto roleUpdateDto);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<IEnumerable<PermissionDto>> GetPermissionsForRoleAsync(int roleId);
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(int roleId); // 注意，返回的是 UserDto
    }
}