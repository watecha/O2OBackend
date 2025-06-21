using O2OBackend.Application.DTOs.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Application.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<PermissionDto?> GetPermissionByCodeAsync(string code);
        Task<int> CreatePermissionAsync(PermissionCreateDto permissionCreateDto);
        Task<bool> UpdatePermissionAsync(PermissionUpdateDto permissionUpdateDto);
        Task<bool> DeletePermissionAsync(int id);
        Task<IEnumerable<PermissionDto>> GetPermissionsForUserAsync(int userId);
    }
}