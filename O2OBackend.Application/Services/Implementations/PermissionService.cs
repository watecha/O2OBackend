using O2OBackend.Application.DTOs.Permission;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // <-- 新增：引入日誌命名空間

namespace O2OBackend.Application.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository; // 用於獲取用戶的權限
        private readonly ILogger<PermissionService> _logger; // <-- 新增：聲明 ILogger 欄位

        // <-- 修改：在建構子中注入 ILogger
        public PermissionService(IPermissionRepository permissionRepository, IUserRepository userRepository, ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _logger = logger; // <-- 賦值
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            _logger.LogInformation("Retrieving all permissions."); // 日誌：開始獲取所有權限
            var permissions = await _permissionRepository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {PermissionCount} permissions.", permissions.Count()); // 日誌：獲取完成
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

        public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve permission with ID: {PermissionId}", id); // 日誌：嘗試獲取權限
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                _logger.LogWarning("Permission with ID: {PermissionId} not found.", id); // 日誌：權限未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved permission with ID: {PermissionId}.", id); // 日誌：獲取成功
            return new PermissionDto
            {
                Id = permission.Id,
                Code = permission.Code,
                Name = permission.Name,
                Description = permission.Description,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
        }

        public async Task<PermissionDto?> GetPermissionByCodeAsync(string code)
        {
            _logger.LogInformation("Attempting to retrieve permission with Code: {PermissionCode}", code); // 日誌：嘗試獲取權限
            var permission = await _permissionRepository.GetByCodeAsync(code);
            if (permission == null)
            {
                _logger.LogWarning("Permission with Code: {PermissionCode} not found.", code); // 日誌：權限未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved permission with Code: {PermissionCode}.", code); // 日誌：獲取成功
            return new PermissionDto
            {
                Id = permission.Id,
                Code = permission.Code,
                Name = permission.Name,
                Description = permission.Description,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
        }

        public async Task<int> CreatePermissionAsync(PermissionCreateDto permissionCreateDto)
        {
            _logger.LogInformation("Attempting to create new permission with Code: {PermissionCode}", permissionCreateDto.Code); // 日誌：嘗試創建權限
            var existingPermission = await _permissionRepository.GetByCodeAsync(permissionCreateDto.Code);
            if (existingPermission != null)
            {
                _logger.LogWarning("Permission creation failed: Permission code '{PermissionCode}' already exists.", permissionCreateDto.Code); // 日誌：權限碼重複
                throw new ApplicationException($"Permission code '{permissionCreateDto.Code}' already exists.");
            }

            var permission = new Permission
            {
                Code = permissionCreateDto.Code,
                Name = permissionCreateDto.Name,
                Description = permissionCreateDto.Description,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var newPermissionId = await _permissionRepository.AddAsync(permission);
            _logger.LogInformation("Successfully created permission with ID: {PermissionId} and Code: {PermissionCode}", newPermissionId, permissionCreateDto.Code); // 日誌：創建成功
            return newPermissionId;
        }

        public async Task<bool> UpdatePermissionAsync(PermissionUpdateDto permissionUpdateDto)
        {
            _logger.LogInformation("Attempting to update permission with ID: {PermissionId}", permissionUpdateDto.Id); // 日誌：嘗試更新權限
            var permission = await _permissionRepository.GetByIdAsync(permissionUpdateDto.Id);
            if (permission == null)
            {
                _logger.LogWarning("Permission update failed: Permission with ID {PermissionId} not found.", permissionUpdateDto.Id); // 日誌：權限不存在
                return false;
            }

            if (permission.Code != permissionUpdateDto.Code)
            {
                var existingPermissionWithNewCode = await _permissionRepository.GetByCodeAsync(permissionUpdateDto.Code);
                if (existingPermissionWithNewCode != null && existingPermissionWithNewCode.Id != permissionUpdateDto.Id)
                {
                    _logger.LogWarning("Permission update failed: Permission code '{PermissionCode}' is already taken by another permission (ID: {ExistingPermissionId}).", permissionUpdateDto.Code, existingPermissionWithNewCode.Id); // 日誌：權限碼衝突
                    throw new ApplicationException($"Permission code '{permissionUpdateDto.Code}' is already taken by another permission.");
                }
            }

            permission.Code = permissionUpdateDto.Code;
            permission.Name = permissionUpdateDto.Name;
            permission.Description = permissionUpdateDto.Description;
            permission.IsActive = permissionUpdateDto.IsActive;
            permission.UpdatedAt = DateTime.Now;

            var updated = await _permissionRepository.UpdateAsync(permission);
            if (updated)
            {
                _logger.LogInformation("Successfully updated permission with ID: {PermissionId}.", permissionUpdateDto.Id); // 日誌：更新成功
            }
            else
            {
                _logger.LogError("Failed to update permission with ID: {PermissionId} in repository.", permissionUpdateDto.Id); // 日誌：更新失敗 (Repository 層)
            }
            return updated;
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            _logger.LogInformation("Attempting to delete permission with ID: {PermissionId}", id); // 日誌：嘗試刪除權限
            var deleted = await _permissionRepository.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted permission with ID: {PermissionId}.", id); // 日誌：刪除成功
            }
            else
            {
                _logger.LogWarning("Permission deletion failed: Permission with ID {PermissionId} not found or could not be deleted.", id); // 日誌：刪除失敗
            }
            return deleted;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsForUserAsync(int userId)
        {
            _logger.LogInformation("Retrieving permissions for user with ID: {UserId}", userId); // 日誌：獲取用戶權限
            var userExists = await _userRepository.GetByIdAsync(userId) != null;
            if (!userExists)
            {
                _logger.LogWarning("Get permissions for user failed: User with ID {UserId} not found.", userId); // 日誌：用戶不存在
                throw new ApplicationException($"User with ID {userId} not found.");
            }

            var permissions = await _permissionRepository.GetPermissionsByUserIdAsync(userId);
            _logger.LogInformation("Successfully retrieved {PermissionCount} permissions for user ID: {UserId}.", permissions.Count(), userId); // 日誌：獲取完成
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
    }
}