using O2OBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(int id);
        Task<Permission?> GetByCodeAsync(string code);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<int> AddAsync(Permission permission);
        Task<bool> UpdateAsync(Permission permission);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(int userId);
    }
}