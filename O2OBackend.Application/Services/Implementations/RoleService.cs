using O2OBackend.Application.DTOs.Role;
using O2OBackend.Application.DTOs.Permission;
using O2OBackend.Application.DTOs.User;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // <-- 新增：引入日誌命名空間

namespace O2OBackend.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RoleService> _logger; // <-- 新增：聲明 ILogger 欄位

        // <-- 修改：在建構子中注入 ILogger
        public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IUserRepository userRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _logger = logger; // <-- 賦值
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            _logger.LogInformation("Retrieving all roles."); // 日誌：開始獲取所有角色
            var roles = await _roleRepository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {RoleCount} roles.", roles.Count()); // 日誌：獲取完成
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve role with ID: {RoleId}", id); // 日誌：嘗試獲取角色
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID: {RoleId} not found.", id); // 日誌：角色未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved role with ID: {RoleId}.", id); // 日誌：獲取成功
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string name)
        {
            _logger.LogInformation("Attempting to retrieve role with Name: {RoleName}", name); // 日誌：嘗試獲取角色
            var role = await _roleRepository.GetByNameAsync(name);
            if (role == null)
            {
                _logger.LogWarning("Role with Name: {RoleName} not found.", name); // 日誌：角色未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved role with Name: {RoleName}.", name); // 日誌：獲取成功
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };
        }

        public async Task<int> CreateRoleAsync(RoleCreateDto roleCreateDto)
        {
            _logger.LogInformation("Attempting to create new role: {RoleName}", roleCreateDto.Name); // 日誌：嘗試創建角色
            var existingRole = await _roleRepository.GetByNameAsync(roleCreateDto.Name);
            if (existingRole != null)
            {
                _logger.LogWarning("Role creation failed: Role name '{RoleName}' already exists.", roleCreateDto.Name); // 日誌：角色名重複
                throw new ApplicationException($"Role name '{roleCreateDto.Name}' already exists.");
            }

            var role = new Role
            {
                Name = roleCreateDto.Name,
                Description = roleCreateDto.Description,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var newRoleId = await _roleRepository.AddAsync(role);
            _logger.LogInformation("Successfully created role with ID: {RoleId} and name: {RoleName}", newRoleId, roleCreateDto.Name); // 日誌：創建成功
            return newRoleId;
        }

        public async Task<bool> UpdateRoleAsync(RoleUpdateDto roleUpdateDto)
        {
            _logger.LogInformation("Attempting to update role with ID: {RoleId}", roleUpdateDto.Id); // 日誌：嘗試更新角色
            var role = await _roleRepository.GetByIdAsync(roleUpdateDto.Id);
            if (role == null)
            {
                _logger.LogWarning("Role update failed: Role with ID {RoleId} not found.", roleUpdateDto.Id); // 日誌：角色不存在
                return false;
            }

            if (role.Name != roleUpdateDto.Name)
            {
                var existingRoleWithNewName = await _roleRepository.GetByNameAsync(roleUpdateDto.Name);
                if (existingRoleWithNewName != null && existingRoleWithNewName.Id != roleUpdateDto.Id)
                {
                    _logger.LogWarning("Role update failed: Role name '{RoleName}' is already taken by another role (ID: {ExistingRoleId}).", roleUpdateDto.Name, existingRoleWithNewName.Id); // 日誌：角色名衝突
                    throw new ApplicationException($"Role name '{roleUpdateDto.Name}' is already taken by another role.");
                }
            }

            role.Name = roleUpdateDto.Name;
            role.Description = roleUpdateDto.Description;
            role.IsActive = roleUpdateDto.IsActive;
            role.UpdatedAt = DateTime.Now;

            var updated = await _roleRepository.UpdateAsync(role);
            if (updated)
            {
                _logger.LogInformation("Successfully updated role with ID: {RoleId}.", roleUpdateDto.Id); // 日誌：更新成功
            }
            else
            {
                _logger.LogError("Failed to update role with ID: {RoleId} in repository.", roleUpdateDto.Id); // 日誌：更新失敗 (Repository 層)
            }
            return updated;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            _logger.LogInformation("Attempting to delete role with ID: {RoleId}", id); // 日誌：嘗試刪除角色
            var deleted = await _roleRepository.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted role with ID: {RoleId}.", id); // 日誌：刪除成功
            }
            else
            {
                _logger.LogWarning("Role deletion failed: Role with ID {RoleId} not found or could not be deleted.", id); // 日誌：刪除失敗
            }
            return deleted;
        }

        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            _logger.LogInformation("Attempting to assign permission ID: {PermissionId} to role ID: {RoleId}.", permissionId, roleId); // 日誌：嘗試分配權限
            var roleExists = await _roleRepository.GetByIdAsync(roleId) != null;
            var permissionExists = await _permissionRepository.GetByIdAsync(permissionId) != null;

            if (!roleExists)
            {
                _logger.LogWarning("Assign permission to role failed: Role with ID {RoleId} not found.", roleId); // 日誌：角色不存在
                throw new ApplicationException($"Role with ID {roleId} not found.");
            }
            if (!permissionExists)
            {
                _logger.LogWarning("Assign permission to role failed: Permission with ID {PermissionId} not found.", permissionId); // 日誌：權限不存在
                throw new ApplicationException($"Permission with ID {permissionId} not found.");
            }

            var assigned = await _roleRepository.AssignPermissionToRoleAsync(roleId, permissionId);
            if (assigned)
            {
                _logger.LogInformation("Successfully assigned permission ID: {PermissionId} to role ID: {RoleId}.", permissionId, roleId); // 日誌：分配成功
            }
            else
            {
                _logger.LogError("Failed to assign permission ID: {PermissionId} to role ID: {RoleId}.", permissionId, roleId); // 日誌：分配失敗
            }
            return assigned;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            _logger.LogInformation("Attempting to remove permission ID: {PermissionId} from role ID: {RoleId}.", permissionId, roleId); // 日誌：嘗試移除權限
            var roleExists = await _roleRepository.GetByIdAsync(roleId) != null;
            var permissionExists = await _permissionRepository.GetByIdAsync(permissionId) != null;

            if (!roleExists)
            {
                _logger.LogWarning("Remove permission from role failed: Role with ID {RoleId} not found.", roleId);
                throw new ApplicationException($"Role with ID {roleId} not found.");
            }
            if (!permissionExists)
            {
                _logger.LogWarning("Remove permission from role failed: Permission with ID {PermissionId} not found.", permissionId);
                throw new ApplicationException($"Permission with ID {permissionId} not found.");
            }

            var removed = await _roleRepository.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (removed)
            {
                _logger.LogInformation("Successfully removed permission ID: {PermissionId} from role ID: {RoleId}.", permissionId, roleId); // 日誌：移除成功
            }
            else
            {
                _logger.LogError("Failed to remove permission ID: {PermissionId} from role ID: {RoleId}.", permissionId, roleId); // 日誌：移除失敗
            }
            return removed;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsForRoleAsync(int roleId)
        {
            _logger.LogInformation("Retrieving permissions for role with ID: {RoleId}", roleId); // 日誌：獲取角色權限
            var permissions = await _roleRepository.GetPermissionsByRoleIdAsync(roleId);
            _logger.LogInformation("Successfully retrieved {PermissionCount} permissions for role ID: {RoleId}.", permissions.Count(), roleId); // 日誌：獲取完成
            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(int roleId)
        {
            _logger.LogInformation("Retrieving users in role with ID: {RoleId}", roleId); // 日誌：獲取角色中的用戶
            var users = await _roleRepository.GetUsersByRoleIdAsync(roleId);
            _logger.LogInformation("Successfully retrieved {UserCount} users in role ID: {RoleId}.", users.Count(), roleId); // 日誌：獲取完成
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
        }
    }
}